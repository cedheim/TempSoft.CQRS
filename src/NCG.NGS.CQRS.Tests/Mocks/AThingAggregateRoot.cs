using System;
using NCG.NGS.CQRS.Commands;
using NCG.NGS.CQRS.Domain;
using NCG.NGS.CQRS.Events;

namespace NCG.NGS.CQRS.Tests.Mocks
{
    public class AThingAggregateRoot : AggregateRoot<AThingAggregateRoot>
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
    }

    public class DoSomething : ICommand
    {
        public int A { get; private set; }
        public string B { get; private set; }

        private DoSomething()
        {
        }

        public DoSomething(int a, string b)
        {
            Id = Guid.NewGuid();
            A = a;
            B = b;
        }

        public Guid Id { get; private set; }
    }

    public class ChangedAValue : IEvent
    {
        private ChangedAValue()
        {
        }

        public ChangedAValue(int a)
        {
            A = a;
            Id = Guid.NewGuid();
        }

        public int A { get; private set; }
        public Guid Id { get; private set; }
        public int Version { get; set; }
        public Guid AggregateRootId { get; set; }
    }

    public class ChangedBValue : IEvent
    {
        private ChangedBValue()
        {
        }

        public ChangedBValue(string b)
        {
            B = b;
            Id = Guid.NewGuid();
        }

        public string B { get; private set; }
        public Guid Id { get; private set; }
        public int Version { get; set; }
        public Guid AggregateRootId { get; set; }
    }
}