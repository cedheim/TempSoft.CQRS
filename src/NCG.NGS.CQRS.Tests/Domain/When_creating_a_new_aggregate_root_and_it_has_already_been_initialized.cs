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
    public class When_creating_a_new_aggregate_root_and_it_has_already_been_initialized
    {
        private AThingAggregateRoot _root;
        private IEvent[] _events;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _root = new AThingAggregateRoot();
            _root.Initialize(Data.RootId);
            _events = _root.Commit().ToArray();
        }

        [Test]
        public void Should_throw_an_already_initialized_exception()
        {
            _root.Invoking(r => r.Initialize(Data.RootId))
                .Should().Throw<InitializationOfAlreadyInitializedAggregateException>();
        }
        
        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
        }

    }
}