using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Mocks;

namespace TempSoft.CQRS.Tests.Domain.AggregateRoot
{
    [TestFixture]
    public class When_creating_a_new_aggregate_root
    {
        private AThingAggregateRoot _root;
        private IEvent[] _events;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _root = new AThingAggregateRoot() {Id = Data.RootId};
            _root.Initialize();
            _events = _root.Commit().Events;
        }

        private static class Data
        {
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
            _events.Should().ContainSingle(e =>
                e is IInitializationEvent && e.Version == 1 && e.EventGroup == nameof(AThingAggregateRoot));
        }

        [Test]
        public void Should_have_updated_the_version()
        {
            _root.Version.Should().Be(1);
        }
    }
}