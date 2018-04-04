using System;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.Domain.AggregateRoot
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
            _events = _root.Commit().Events;
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