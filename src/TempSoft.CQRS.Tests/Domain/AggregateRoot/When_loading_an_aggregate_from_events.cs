using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Mocks;

namespace TempSoft.CQRS.Tests.Domain.AggregateRoot
{
    [TestFixture]
    public class When_loading_an_aggregate_from_events
    {
        private AThingAggregateRoot _root;
        private IEvent[] _events;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var original = new AThingAggregateRoot() {Id = Data.RootId};
            original.Initialize();

            await original.Handle(new DoSomething(Data.AValue, Data.BValue), CancellationToken.None);
            _events = original.Commit().Events;

            _root = new AThingAggregateRoot() {Id = Data.RootId};
            _root.LoadFrom(_events, Enumerable.Empty<Guid>());
        }

        private static class Data
        {
            public const int AValue = 5;
            public const string BValue = "FLEUF";
            public static readonly Guid RootId = Guid.NewGuid();
        }

        [Test]
        public void Should_have_set_the_id()
        {
            _root.Id.Should().Be(Data.RootId);
        }

        [Test]
        public void Should_have_triggered_an_event()
        {
            _events.Should().ContainSingle(e => e is IInitializationEvent && e.Version == 1);
        }

        [Test]
        public void Should_have_triggered_events()
        {
            _events.Should().ContainSingle(e => e is ChangedAValue);
            _events.Should().ContainSingle(e => e is ChangedBValue);
        }

        [Test]
        public void Should_have_updated_the_values()
        {
            _root.A.Should().Be(Data.AValue);
            _root.B.Should().Be(Data.BValue);
        }

        [Test]
        public void Should_have_updated_version()
        {
            _root.Version.Should().Be(3);
        }
    }
}