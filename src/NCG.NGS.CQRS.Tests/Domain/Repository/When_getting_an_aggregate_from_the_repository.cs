using System;
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
    public class When_getting_an_aggregate_from_the_repository
    {
        private IEventStore _eventStore;
        private IEventBus _eventBus;
        private ICommandRegistry _commandRegistry;
        private CQRS.Domain.AggregateRootRepository _aggregateRootRepository;
        private AThingAggregateRoot _root;

        [SetUp]
        public async Task OneTimeSetUp()
        {
            _eventStore = A.Fake<IEventStore>();
            _eventBus = A.Fake<IEventBus>();
            _commandRegistry = A.Fake<ICommandRegistry>();

            A.CallTo(() => _eventStore.Get(A<Guid>.Ignored, A<int>.Ignored, A<CancellationToken>.Ignored))
                .Returns(new IEvent[] { new CreatedAThing(Data.AggregateRootId) { Version = 1 }, new ChangedAValue(Data.AValue) { Version = 2 }, new ChangedBValue(Data.BValue) { Version = 3 }, });

            _aggregateRootRepository = new CQRS.Domain.AggregateRootRepository(_eventStore, _eventBus, _commandRegistry);
            
            _root = await _aggregateRootRepository.Get<AThingAggregateRoot>(Data.AggregateRootId, CancellationToken.None);

        }
        
        [Test]
        public void Should_initialize_with_events()
        {
            _root.Id.Should().Be(Data.AggregateRootId);
            _root.A.Should().Be(Data.AValue);
            _root.B.Should().Be(Data.BValue);
        }

        [Test]
        public void Should_have_tried_to_load_from_the_event_store()
        {
            A.CallTo(() => _eventStore.Get(Data.AggregateRootId, 0, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_tried_to_load_from_the_command_registry()
        {
            A.CallTo(() => _commandRegistry.Get(Data.AggregateRootId, A<CancellationToken>.Ignored))
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