using System;
using System.Linq;
using FluentAssertions;
using NCG.NGS.CQRS.Events;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.Domain
{
    [TestFixture]
    public class When_creating_a_new_aggregate_root
    {
        private AThingAggregateRoot _root;
        private IEvent[] _events;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _root = new AThingAggregateRoot();
            _root.Initialize(Data.RootId);
            _events = _root.Commit().Events;
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
        public void Should_have_updated_the_version()
        {
            _root.Version.Should().Be(1);
        }

        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
        }

    }
}