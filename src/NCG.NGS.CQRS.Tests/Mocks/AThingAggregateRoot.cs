using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NCG.NGS.CQRS.Commands;
using NCG.NGS.CQRS.Domain;
using NCG.NGS.CQRS.Events;
using NCG.NGS.CQRS.Queries;

namespace NCG.NGS.CQRS.Tests.Mocks
{
    public class AThingAggregateRoot : AggregateRoot<AThingAggregateRoot>, IAggregateRootWithReadModel
    {
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

        public override void Initialize(Guid id)
        {
            ApplyChange(new CreatedAThing(id));
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