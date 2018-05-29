using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using TempSoft.CQRS.Common.Extensions;
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Mocks;

namespace TempSoft.CQRS.CosmosDb.Tests.Events.EventStore
{
    [TestFixture]
    [Category("Integration")]
    public class IntegrationTests
    {
        private CosmosDbEventStore _repository;
        private DocumentClient _client;
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

            _repository = new CosmosDbEventStore(_client, new CosmosDbQueryPager(), Data.DatabaseId, Data.Collectionid);

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

            await Task.WhenAll(_repository.Save(events1), _repository.Save(events2), _repository.Save(events3));

            _events = events1.Concat(events2).Concat(events3).ToArray();
        }

        private static class Data
        {
            public const string DatabaseId = "tempsoft_cqrs_tests";
            public const string Collectionid = "events";
            public const string DatabaseLink = "database";
            public const int AValue = 5;
            public const string BValue = "DOH";

            public static readonly Guid AggregateRootId1 = Guid.NewGuid();
            public static readonly Guid AggregateRootId2 = Guid.NewGuid();
            public static readonly Guid AggregateRootId3 = Guid.NewGuid();
        }

        [Test]
        public async Task When_creating_the_repository()
        {
            var database = _client.CreateDatabaseQuery().Where(d => d.Id == Data.DatabaseId).ToArray().FirstOrDefault();
            database.Should().NotBeNull();

            var collections = await _client.ReadDocumentCollectionFeedAsync(database.SelfLink);
            var collection = collections.Where(coll => coll.Id == Data.Collectionid).ToArray().FirstOrDefault();

            collection.Should().NotBeNull();
        }

        [Test]
        public async Task When_getting_multiple_documents()
        {
            var result = (await _repository.Get(Data.AggregateRootId1)).ToArray();

            result.Should().BeInAscendingOrder(e => e.Version);
            result.Should().OnlyContain(e => e.AggregateRootId == Data.AggregateRootId1);
            result.Should().HaveCount(_events.Count(e => e.AggregateRootId == Data.AggregateRootId1));
        }

        [Test]
        public async Task When_listing_documents()
        {
            var result = new List<IEvent>();
            await _repository.List(
                new EventStoreFilter
                {
                    EventGroups = new[] {nameof(AThingAggregateRoot)},
                    EventTypes = new[] {typeof(ChangedBValue) }
                }, (e, token) => Task.Run(() => result.Add(e), token));

            result.Should().HaveCountGreaterThan(0);
            result.Should().AllBeOfType<ChangedBValue>();
            result.Should().OnlyContain(e => e.EventGroup == nameof(AThingAggregateRoot));
        }

        [Test]
        public async Task When_saveing_a_document()
        {
            var @event = _events.FirstOrDefault();
            var result = _client.CreateDocumentQuery<EventPayloadWrapper>(
                UriFactory.CreateDocumentCollectionUri(Data.DatabaseId, Data.Collectionid), new FeedOptions
                {
                    PartitionKey = new PartitionKey(@event.AggregateRootId.ToString()),
                    EnableCrossPartitionQuery = false,
                    MaxDegreeOfParallelism = -1,
                    MaxBufferedItemCount = -1
                }).Where(e => e.Id == @event.Id).ToArray().FirstOrDefault();

            result.GetEvent().Should().BeEquivalentTo(@event);
        }
    }
}