using System;
using System.Fabric;
using System.Fabric.Health;
using System.Fabric.Query;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using ServiceFabric.Mocks;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;
using TempSoft.CQRS.ServiceFabric.Tools;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Events.EventStreamService
{
    public abstract class EventStreamServiceTestBase
    {
        protected const string EventStreamName = "EVENT STREAM";
        protected const string ServiceType = "EventStreamServiceType";
        protected static readonly Uri ServiceUri = new Uri($"fabric:/TempSoft.Cqrs.Application/{ServiceType}");
        protected static readonly Guid PartitionId = Guid.NewGuid();
        protected readonly IEventStore EventStore = A.Fake<IEventStore>();

        protected readonly EventStreamDefinition EventStreamDefinition = new EventStreamDefinition(EventStreamName,
            new EventFilter
            {
                EventGroups = new[] {nameof(AThingAggregateRoot)},
                EventTypes = new[] {typeof(ChangedAValue)}
            });

        protected readonly IEventStreamRegistry EventStreamRegistry = A.Fake<IEventStreamRegistry>();
        protected readonly IFabricClient FabricClient = A.Fake<IFabricClient>();
        protected readonly IQueryClient QueryClient = A.Fake<IQueryClient>();

        protected readonly IEventStreamService Service;
        protected readonly MockReliableStateManager StateManager = new MockReliableStateManager();
        protected readonly IEventStreamStateManager StreamStateManager = A.Fake<IEventStreamStateManager>();


        protected EventStreamServiceTestBase()
        {
            var context = MockStatefulServiceContextFactory.Create(MockCodePackageActivationContext.Default,
                ServiceType, ServiceUri, PartitionId, 0);
            var partitionInformation = MockQueryPartitionFactory.CreateStatefulPartition(
                MockQueryPartitionFactory.CreateNamedPartitonInfo(EventStreamName, context.PartitionId), 1, 1,
                HealthState.Ok, ServicePartitionStatus.Ready, TimeSpan.MaxValue, new Epoch());

            A.CallTo(() => EventStreamRegistry.GetEventStreamByName(A<string>.Ignored))
                .Returns(EventStreamDefinition);
            A.CallTo(() => FabricClient.QueryManager)
                .Returns(QueryClient);
            A.CallTo(() => QueryClient.GetPartitionAsync(A<Guid>.Ignored))
                .ReturnsLazily(foc => Task.FromResult(new ServicePartitionList {partitionInformation}));

            Service = new CQRS.ServiceFabric.Events.EventStreamService(context, StateManager, StreamStateManager,
                FabricClient, EventStore, EventStreamRegistry);

            ((Task) Service.CallPrivateMethod("OnOpenAsync", ReplicaOpenMode.New, CancellationToken.None)).Wait();
        }
    }
}