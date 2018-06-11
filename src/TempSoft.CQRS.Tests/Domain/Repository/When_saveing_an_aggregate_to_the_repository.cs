using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.Mocks;

namespace TempSoft.CQRS.Tests.Domain.Repository
{
    [TestFixture]
    public class When_saveing_an_aggregate_to_the_repository
    {
        private IEventStore _eventStore;
        private IEventBus _eventBus;
        private ICommandRegistry _commandRegistry;
        private AggregateRootRepository _aggregateRootRepository;
        private AThingAggregateRoot _root;
        private IServiceProvider _serviceProvider;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _eventStore = A.Fake<IEventStore>();
            _eventBus = A.Fake<IEventBus>();
            _commandRegistry = A.Fake<ICommandRegistry>();
            _serviceProvider = new FluentBootstrapper();

            _aggregateRootRepository = new AggregateRootRepository(_eventStore, _eventBus, _commandRegistry, _serviceProvider);

            _root = new AThingAggregateRoot() {Id = Data.AggregateRootId};
            _root.Initialize();
            await _root.Handle(new DoSomething(Data.AValue, Data.BValue), CancellationToken.None);

            await _aggregateRootRepository.Save(_root, CancellationToken.None);
        }


        private static class Data
        {
            public const int AValue = 5;
            public const string BValue = "HELLU";
            public static readonly Guid AggregateRootId = Guid.NewGuid();
        }

        [Test]
        public void Should_have_published_the_events()
        {
            A.CallTo(() => _eventBus.Publish(A<IEnumerable<IEvent>>.That.Matches(es =>
                    es.Any(e => e is IInitializationEvent) &&
                    es.Any(e => e is ChangedAValue) &&
                    es.Any(e => e is ChangedBValue)), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_saved_the_events()
        {
            A.CallTo(() =>
                    _eventStore.Save(
                        A<IEnumerable<IEvent>>.That.Matches(es =>
                            es.Any(e => e is IInitializationEvent) &&
                            es.Any(e => e is ChangedAValue) &&
                            es.Any(e => e is ChangedBValue)
                        ), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_updated_the_command_registry()
        {
            A.CallTo(() => _commandRegistry.Save(Data.AggregateRootId,
                    A<IEnumerable<Guid>>.That.Matches(ids => ids.Any()), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}