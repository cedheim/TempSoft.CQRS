using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NCG.NGS.CQRS.Commands;
using NCG.NGS.CQRS.Events;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.Domain.Repository
{
    [TestFixture]
    public class When_saveing_an_aggregate_to_the_repository
    {
        private IEventStore _eventStore;
        private IEventBus _eventBus;
        private ICommandRegistry _commandRegistry;
        private CQRS.Domain.AggregateRootRepository _aggregateRootRepository;
        private AThingAggregateRoot _root;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _eventStore = A.Fake<IEventStore>();
            _eventBus = A.Fake<IEventBus>();
            _commandRegistry = A.Fake<ICommandRegistry>();

            _aggregateRootRepository = new CQRS.Domain.AggregateRootRepository(_eventStore, _eventBus, _commandRegistry);

            _root = new AThingAggregateRoot();
            _root.Initialize(Data.AggregateRootId);
            _root.Handle(new DoSomething(Data.AValue, Data.BValue));

            await _aggregateRootRepository.Save(_root, CancellationToken.None);
        }

        [Test]
        public void Should_have_saved_the_events()
        {
            A.CallTo(() =>
                    _eventStore.Save(Data.AggregateRootId,
                        A<IEnumerable<IEvent>>.That.Matches(es =>
                            es.Any(e => e is InitializationEvent) && 
                            es.Any(e => e is ChangedAValue) &&
                            es.Any(e => e is ChangedBValue)
                        ), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_updated_the_command_registry()
        {
            A.CallTo(() => _commandRegistry.Save(Data.AggregateRootId, A<IEnumerable<Guid>>.That.Matches(ids => ids.Any()), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_published_the_events()
        {
            A.CallTo(() => _eventBus.Publish(A<IEnumerable<IEvent>>.That.Matches(es =>
                    es.Any(e => e is InitializationEvent) &&
                    es.Any(e => e is ChangedAValue) &&
                    es.Any(e => e is ChangedBValue)), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }


        private static class Data
        {
            public static readonly Guid AggregateRootId = Guid.NewGuid();
            public const int AValue = 5;
            public const string BValue = "HELLU";
        }
    }
}