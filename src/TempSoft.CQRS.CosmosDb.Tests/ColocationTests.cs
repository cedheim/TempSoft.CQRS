using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using TempSoft.CQRS.CosmosDb.Commands;
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.CosmosDb.Projectors;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.CosmosDb.Tests
{
    [TestFixture]
    [Category("Integration")]
    public class ColocationTests
    {
        private DocumentClient _client;
        private CosmosDbProjectionModelRepository _projectionRepository;
        private CosmosDbCommandRegistry _commandRepository;
        private CosmosDbEventStore _eventRepository;
        private IProjection[] _projections;
        private IEvent[] _events;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            // Connect to the Azure Cosmos DB Emulator running locally
            _client = new DocumentClient(new Uri("https://localhost:8081"),
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            await _client.OpenAsync();

            var database = _client.CreateDatabaseQuery().Where(d => d.Id == Data.DatabaseId).ToArray().FirstOrDefault();
            if (!ReferenceEquals(database, default(Database))) await _client.DeleteDatabaseAsync(database.SelfLink);


            _projectionRepository = new CosmosDbProjectionModelRepository(_client, new CosmosDbQueryPager(), Data.DatabaseId, Data.Collectionid);
            _commandRepository = new CosmosDbCommandRegistry(_client, new CosmosDbQueryPager(), Data.DatabaseId, Data.Collectionid);
            _eventRepository = new CosmosDbEventStore(_client, new CosmosDbQueryPager(), Data.DatabaseId, Data.Collectionid);

            await Task.WhenAll(SetUpProjections(), SetUpCommands(), SetUpEvents());
        }

        [Test]
        public async Task When_getting_a_projection()
        {
            var result = await _projectionRepository.Get<AThingProjection>(Data.ProjectionId1, Data.ProjectorId1, CancellationToken.None);
            result.Should().BeEquivalentTo(_projections[0]);
        }


        [Test]
        public async Task When_listing_projections()
        {
            var result = new List<IProjection>();
            await _projectionRepository.List(Data.ProjectorId1, (projection, token) => Task.Run(() => result.Add(projection)), CancellationToken.None);

            result.Should().HaveCount(_projections.Count(p => p.ProjectorId == Data.ProjectorId1));
        }


        [Test]
        public async Task When_listing_events()
        {
            var result = new List<IEvent>();
            await _eventRepository.List(
                new EventStoreFilter
                {
                    EventGroups = new[] { nameof(AThingAggregateRoot) },
                    EventTypes = new[] { typeof(ChangedBValue) }
                }, (e, token) => Task.Run(() => result.Add(e), token));

            result.Should().HaveCountGreaterThan(0);
            result.Should().AllBeOfType<ChangedBValue>();
            result.Should().OnlyContain(e => e.EventGroup == nameof(AThingAggregateRoot));
        }


        private async Task SetUpProjections()
        {
            _projections = new IProjection[]
            {
                new AThingProjection(Data.ProjectionId1, Data.ProjectorId1) { A = Data.AValue, B = Data.BValue },
                new AThingProjection(Data.ProjectionId2, Data.ProjectorId1) { A = Data.AValue, B = Data.BValue },
                new AThingProjection(Data.ProjectionId3, Data.ProjectorId1) { A = Data.AValue, B = Data.BValue },
                new AThingProjection(Data.ProjectionId4, Data.ProjectorId1) { A = Data.AValue, B = Data.BValue },
                new AThingProjection(Data.ProjectionId5, Data.ProjectorId2) { A = Data.AValue, B = Data.BValue },
                new AThingProjection(Data.ProjectionId6, Data.ProjectorId2) { A = Data.AValue, B = Data.BValue },
                new AThingProjection(Data.ProjectionId1, Data.ProjectorId2) { A = Data.AValue, B = Data.BValue },
            };

            await Task.WhenAll(_projections.Select(p => _projectionRepository.Save(p, CancellationToken.None)));
        }

        private async Task SetUpCommands()
        {
            await _commandRepository.Save(Data.AggregateRootId1, Data.CommandIds1);
            await _commandRepository.Save(Data.AggregateRootId2, Data.CommandIds2);
            await _commandRepository.Save(Data.AggregateRootId4, Data.CommandIds4);
        }

        private async Task SetUpEvents()
        {

            var events1 = new IEvent[]
            {
                new ChangedAValue(Data.AValue)
                {
                    AggregateRootId = Data.AggregateRootId1,
                    Version = 3,
                    EventGroup = nameof(AThingAggregateRoot)
                },
                new ChangedBValue(Data.BValue)
                {
                    AggregateRootId = Data.AggregateRootId1,
                    Version = 1,
                    EventGroup = nameof(AThingAggregateRoot)
                },
                new ChangedAValue(Data.AValue)
                {
                    AggregateRootId = Data.AggregateRootId1,
                    Version = 2,
                    EventGroup = nameof(AThingAggregateRoot)
                },
                new ChangedAValue(Data.AValue)
                {
                    AggregateRootId = Data.AggregateRootId1,
                    Version = 0,
                    EventGroup = nameof(AThingAggregateRoot)
                }
            };
            var events2 = new IEvent[]
            {
                new ChangedAValue(Data.AValue)
                {
                    AggregateRootId = Data.AggregateRootId2,
                    Version = 3,
                    EventGroup = nameof(AThingAggregateRoot)
                },
                new ChangedAValue(Data.AValue)
                {
                    AggregateRootId = Data.AggregateRootId2,
                    Version = 1,
                    EventGroup = nameof(AThingAggregateRoot)
                },
                new ChangedAValue(Data.AValue)
                {
                    AggregateRootId = Data.AggregateRootId2,
                    Version = 2,
                    EventGroup = nameof(AThingAggregateRoot)
                },
                new ChangedBValue(Data.BValue)
                {
                    AggregateRootId = Data.AggregateRootId2,
                    Version = 0,
                    EventGroup = nameof(AThingAggregateRoot)
                }
            };
            var events3 = new IEvent[]
            {
                new ChangedAValue(Data.AValue)
                {
                    AggregateRootId = Data.AggregateRootId3,
                    Version = 3,
                    EventGroup = "AA"
                },
                new ChangedAValue(Data.AValue)
                {
                    AggregateRootId = Data.AggregateRootId3,
                    Version = 1,
                    EventGroup = "AA"
                },
                new ChangedAValue(Data.AValue)
                {
                    AggregateRootId = Data.AggregateRootId3,
                    Version = 2,
                    EventGroup = "AA"
                },
                new ChangedBValue(Data.BValue) {AggregateRootId = Data.AggregateRootId3, Version = 0, EventGroup = "AA"}
            };

            await Task.WhenAll(_eventRepository.Save(events1), _eventRepository.Save(events2), _eventRepository.Save(events3));

            _events = events1.Concat(events2).Concat(events3).ToArray();
        }


        private static class Data
        {
#if NETCOREAPP2_1
            public const string DatabaseId = "tempsoft_cqrs_tests_core";
#else
            public const string DatabaseId = "tempsoft_cqrs_tests_net452";
#endif
            public const string Collectionid = "cqrs_store";
            public const string DatabaseLink = "database";
            public const int AValue = 5;
            public const string BValue = "DOH";

            public static readonly string ProjectorId1 = Guid.NewGuid().ToString();
            public static readonly string ProjectorId2 = Guid.NewGuid().ToString();
            public static readonly string ProjectorId3 = Guid.NewGuid().ToString();

            public static readonly string ProjectionId1 = Guid.NewGuid().ToString();
            public static readonly string ProjectionId2 = Guid.NewGuid().ToString();
            public static readonly string ProjectionId3 = Guid.NewGuid().ToString();
            public static readonly string ProjectionId4 = Guid.NewGuid().ToString();
            public static readonly string ProjectionId5 = Guid.NewGuid().ToString();
            public static readonly string ProjectionId6 = Guid.NewGuid().ToString();


            public static readonly string AggregateRootId1 = Guid.NewGuid().ToString();
            public static readonly string AggregateRootId2 = Guid.NewGuid().ToString();
            public static readonly string AggregateRootId3 = Guid.NewGuid().ToString();
            public static readonly string AggregateRootId4 = "öåöAS!\"#¤%&/()=?`";

            public static readonly Guid[] CommandIds1 = { Guid.NewGuid() };

            public static readonly Guid[] CommandIds2 =
                {Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};

            public static readonly Guid[] CommandIds4 = { Guid.NewGuid() };
        }

    }
}