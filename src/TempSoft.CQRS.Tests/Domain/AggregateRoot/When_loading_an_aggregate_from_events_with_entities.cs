using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Mocks;

namespace TempSoft.CQRS.Tests.Domain.AggregateRoot
{
    [TestFixture]
    public class When_loading_an_aggregate_from_events_with_entities
    {
        private AThingAggregateRoot _root;
        private IEvent[] _events;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _events = new IEvent[]
            {
                new CreatedAThing() {Version = 1},
                new AddedStuff(Data.EntityId, Data.StuffMessage) {AggregateRootId = Data.RootId, Version = 2},
                new StuffMessageSet(Data.EntityId, Data.ChangedStuffMessage)
                {
                    AggregateRootId = Data.RootId,
                    Version = 3
                }
            };

            _root = new AThingAggregateRoot();
            _root.LoadFrom(_events, Enumerable.Empty<Guid>());
        }

        private static class Data
        {
            public const string StuffMessage = "STUFF!!";
            public const string ChangedStuffMessage = "MOAR STUFF!!!!";
            public static readonly Guid RootId = Guid.NewGuid();
            public static readonly Guid EntityId = Guid.NewGuid();
        }

        [Test]
        public void Should_have_added_and_updated_entity()
        {
            _root.Stuff.Should()
                .ContainSingle(stuff => stuff.Id == Data.EntityId && stuff.Message == Data.ChangedStuffMessage);
        }
    }
}