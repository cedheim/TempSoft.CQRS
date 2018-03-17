using System;
using System.Linq;
using FluentAssertions;
using NCG.NGS.CQRS.Events;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.Domain.AggregateRoot
{
    [TestFixture]
    public class When_loading_an_aggregate_from_events
    {
        private AThingAggregateRoot _root;
        private IEvent[] _events;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var original = new AThingAggregateRoot();
            original.Initialize(Data.RootId);
            original.Handle(new DoSomething(Data.AValue, Data.BValue));
            _events = original.Commit().Events;

            _root = new AThingAggregateRoot();
            _root.LoadFrom(_events, Enumerable.Empty<Guid>());
        }

        [Test]
        public void Should_have_set_the_id()
        {
            _root.Id.Should().Be(Data.RootId);
        }

        [Test]
        public void Should_have_triggered_an_event()
        {
            _events.Should().ContainSingle(e => e is InitializationEvent && e.Version == 1);
        }

        [Test]
        public void Should_have_updated_the_values()
        {
            _root.A.Should().Be(Data.AValue);
            _root.B.Should().Be(Data.BValue);
        }

        [Test]
        public void Should_have_triggered_events()
        {
            _events.Should().ContainSingle(e => e is ChangedAValue);
            _events.Should().ContainSingle(e => e is ChangedBValue);
        }

        [Test]
        public void Should_have_updated_version()
        {
            _root.Version.Should().Be(3);
        }

        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
            public const int AValue = 5;
            public const string BValue = "FLEUF";
        }
    }
}