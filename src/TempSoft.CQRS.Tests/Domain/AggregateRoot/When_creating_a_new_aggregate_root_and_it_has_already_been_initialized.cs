using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Exceptions;
using TempSoft.CQRS.Mocks;

namespace TempSoft.CQRS.Tests.Domain.AggregateRoot
{
    [TestFixture]
    public class When_creating_a_new_aggregate_root_and_it_has_already_been_initialized
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
        public void Should_throw_an_already_initialized_exception()
        {
            _root.Invoking(r => r.Initialize())
                .Should().Throw<InitializationOfAlreadyInitializedAggregateException>();
        }
    }
}