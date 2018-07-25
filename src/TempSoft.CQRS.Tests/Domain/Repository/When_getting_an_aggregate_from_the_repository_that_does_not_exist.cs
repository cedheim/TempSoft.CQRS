using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.Mocks;

namespace TempSoft.CQRS.Tests.Domain.Repository
{
    [TestFixture]
    public class When_getting_an_aggregate_from_the_repository_that_does_not_exist
    {
        private IEventStore _eventStore;
        private IEventBus _eventBus;
        private ICommandRegistry _commandRegistry;
        private IServiceProvider _serviceProvider;
        private AggregateRootRepository _aggregateRootRepository;
        private AThingAggregateRoot _root;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _eventStore = A.Fake<IEventStore>();
            _eventBus = A.Fake<IEventBus>();
            _commandRegistry = A.Fake<ICommandRegistry>();
            _serviceProvider = new FluentBootstrapper();

            _aggregateRootRepository = new AggregateRootRepository(_eventStore, _eventBus, _commandRegistry, _serviceProvider);
            _root = await _aggregateRootRepository.Get<AThingAggregateRoot>(Data.AggregateRootId,true, CancellationToken.None);
        }

        private static class Data
        {
            public static readonly Guid AggregateRootId = Guid.NewGuid();
        }

        [Test]
        public void Should_create_an_uninitialized_instance()
        {
            _root.Should().NotBeNull();
        }

        [Test]
        public void Should_have_tried_to_load_from_the_command_registry()
        {
            A.CallTo(() => _commandRegistry.Get(Data.AggregateRootId, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_tried_to_load_from_the_event_store()
        {
            A.CallTo(() => _eventStore.Get(Data.AggregateRootId, 0, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}