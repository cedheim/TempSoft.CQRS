using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Queries;

namespace TempSoft.CQRS.Tests.Mocks
{
    public class AThingAggregateRoot : AggregateRoot<AThingAggregateRoot>, IAggregateRootWithReadModel
    {
        private readonly List<StuffEntity> _stuff = new List<StuffEntity>();

        public IEnumerable<StuffEntity> Stuff => _stuff;
        public int A { get; private set; }
        public string B { get; private set; }

        [CommandHandler(typeof(DoSomething))]
        public void DoSomething(int a, string b)
        {
            ApplyChange(new ChangedAValue(a));
            ApplyChange(new ChangedBValue(b));
        }

        [EventHandler(typeof(ChangedAValue))]
        private void Apply(ChangedAValue @event)
        {
            A = @event.A;
        }

        [EventHandler(typeof(ChangedBValue))]
        private void Apply(ChangedBValue @event)
        {
            B = @event.B;
        }

        [EventHandler(typeof(CreatedAThing))]
        private void Apply(CreatedAThing @event)
        {
            Id = @event.AggregateRootId;
        }

        [CommandHandler(typeof(InitializeAThing))]
        public void Initialize(Guid aggregateRootId)
        {
            ApplyChange(new CreatedAThing(aggregateRootId));
        }

        [CommandHandler(typeof(AddStuff))]
        public void AddStuff(Guid entityId, string message)
        {
            ApplyChange(new AddedStuff(entityId, message));
        }

        [EventHandler(typeof(AddedStuff))]
        public void Apply(AddedStuff @event)
        {
            var entity = new StuffEntity(this, @event.EntityId, @event.Message);
            _stuff.Add(entity);
        }

        public IAggregateRootReadModel GetReadModel()
        {
            return new AThingReadModel
            {
                A = A,
                B = B,
                Version = Version,
                Id = Id
            };
        }
    }

    public class StuffEntity : AThingAggregateRoot.Entity<StuffEntity>
    {
        public StuffEntity(AThingAggregateRoot root, Guid id, string message) : base(root, id)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }

    public class InitializeAThing : CommandBase
    {
        private InitializeAThing() { }

        public InitializeAThing(Guid aggregateRootId)
        {
            AggregateRootId = aggregateRootId;
        }

        public Guid AggregateRootId { get; private set; }
    }

    public class AddStuff : CommandBase
    {
        private AddStuff() { }

        public AddStuff(Guid entityId, string message)
        {
            Message = message;
            EntityId = entityId;
        }

        public string Message { get; private set; }

        public Guid EntityId { get; private set; }
    }

    public class DoSomething : CommandBase
    {
        private DoSomething() { }

        public DoSomething(int a, string b) { A = a; B = b; }

        public int A { get; private set; }
        public string B { get; private set; }
    }

    public class ChangedAValue : EventBase
    {
        private ChangedAValue() { }

        public ChangedAValue(int a) { A = a; }

        public int A { get; private set; }
    }

    public class ChangedBValue : EventBase
    {
        private ChangedBValue() { }

        public ChangedBValue(string b) { B = b; }

        public string B { get; private set; }
    }

    public class CreatedAThing : InitializationEventBase
    {
        private CreatedAThing()
        {
        }

        public CreatedAThing(Guid aggregateRootId) 
            : base(aggregateRootId)
        {
        }
    }

    public class AddedStuff : EventBase
    {
        private AddedStuff() { }

        public AddedStuff(Guid entityId, string message)
        {
            EntityId = entityId;
            Message = message;
        }

        public Guid EntityId { get; private set; }
        public string Message { get; private set; }
    }
    
    public class AThingQueryBuilder : QueryBuilderBase<AThingQueryBuilder>
    {
        private static readonly Type[] AThingEvents = {
            typeof(CreatedAThing),
            typeof(ChangedAValue),
            typeof(ChangedBValue)
        };

        public override IEnumerable<Type> Events => AThingEvents;

        public AThingQueryBuilder(IQueryModelRepository repository) : base(repository)
        {
        }

        [QueryBuilderHandler(typeof(CreatedAThing))]
        public async Task Apply(CreatedAThing @event, CancellationToken cancellationToken)
        {
            await Repository.Save(@event.AggregateRootId.ToString(), new AThingQueryModel(), cancellationToken);
        }

        [QueryBuilderHandler(typeof(ChangedAValue))]
        public async Task Apply(Guid aggregateRootId, int a, CancellationToken cancellationToken)
        {
            var id = aggregateRootId.ToString();
            var model = await Repository.Get<AThingQueryModel>(id, cancellationToken);
            model.A = a;
            await Repository.Save(id, model, cancellationToken);
        }

        [QueryBuilderHandler(typeof(ChangedBValue))]
        public async Task Apply(ChangedBValue @event, CancellationToken cancellationToken)
        {
            var id = @event.AggregateRootId.ToString();
            var model = await Repository.Get<AThingQueryModel>(id, cancellationToken);
            model.B = @event.B;
            await Repository.Save(id, model, cancellationToken);
        }
    }

    public class AThingQueryModel
    {
        public int A { get; set; }

        public string B { get; set; }
    }

    public class AThingReadModel : IAggregateRootReadModel
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public int A { get; set; }

        public string B { get; set; }
    }

}