using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.InMemory.Infrastructure;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Tests.Infrastructure
{
    [TestFixture]
    public class When_working_with_in_memory_services
    {
        private FluentBootstrapper _bootstrapper;
        private ICommandRouter _commandRouter;
        private IProjectionModelRepository _projectionModelRepository;
        private IEventStore _eventStore;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _bootstrapper = new FluentBootstrapper();
            _bootstrapper.UseInMemoryCommandRegistry()
                .UseInMemoryCommandRouter()
                .UseInMemoryEventBus()
                .UseInMemoryEventStore()
                .UseInMemoryProjectionModelRepository()
                .UseProjector<AThingProjector>(nameof(AThingProjector))
                .Validate();

            _commandRouter = _bootstrapper.Resolve<ICommandRouter>();
            _projectionModelRepository = _bootstrapper.Resolve<IProjectionModelRepository>();
            _eventStore = _bootstrapper.Resolve<IEventStore>();

            await _commandRouter.Handle<AThingAggregateRoot>(Data.RootId, new InitializeAThing(Data.RootId), CancellationToken.None);
            await _commandRouter.Handle<AThingAggregateRoot>(Data.RootId, new DoSomething(Data.AValue, Data.BValue), CancellationToken.None);
            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _bootstrapper.Dispose();
        }

        [Test]
        public async Task Should_have_created_a_projection()
        {
            var id = $"{nameof(AThingProjection)}_{Data.RootId}";
            var projection = await _projectionModelRepository.Get<AThingProjection>(id, nameof(AThingProjector), CancellationToken.None);

            projection.Should().NotBeNull();
            projection.A.Should().Be(Data.AValue);
            projection.B.Should().Be(Data.BValue);
        }

        [Test]
        public async Task Should_have_stored_events_in_the_event_store()
        {
            var events = (await _eventStore.Get(Data.RootId)).ToArray();

            events.Should().ContainSingle(e => e is CreatedAThing);
            events.Should().ContainSingle(e => e is ChangedAValue);
            events.Should().ContainSingle(e => e is ChangedBValue);
        }

        private static class Data
        {
            public const int AValue = 5;
            public const string BValue = "HELLO WORLD?";

            public static readonly Guid RootId = Guid.NewGuid();
        }
    }
}