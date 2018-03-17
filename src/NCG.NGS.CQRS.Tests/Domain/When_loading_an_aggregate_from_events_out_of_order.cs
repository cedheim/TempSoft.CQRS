using System;
using System.Linq;
using FluentAssertions;
using NCG.NGS.CQRS.Events;
using NCG.NGS.CQRS.Exception;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.Domain
{
    [TestFixture]
    public class When_loading_an_aggregate_from_events_out_of_order
    {

        private AThingAggregateRoot _root;
        private IEvent[] _events;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _events = new IEvent[] { new InitializationEvent(Data.RootId) { Version = 1 }, new ChangedAValue(Data.AValue) { Version = 3 }, new ChangedBValue(Data.BValue) { Version = 2 } };

            _root = new AThingAggregateRoot();
        }

        [Test]
        public void Should_throw_an_out_of_order_exception()
        {
            _root.Invoking(r => r.LoadFrom(_events, Enumerable.Empty<Guid>()))
                .Should().Throw<EventsOutOfOrderException>();
        }

        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
            public const int AValue = 5;
            public const string BValue = "FLEUF";
        }
    }
}