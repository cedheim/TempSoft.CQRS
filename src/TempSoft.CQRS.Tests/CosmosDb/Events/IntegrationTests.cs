using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.CosmosDb.Events
{
    [TestFixture]
    [Category("Integration")]
    public class IntegrationTests
    {
        private CosmosDbEventStore _repository;
        private DocumentClient _client;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            // Connect to the Azure Cosmos DB Emulator running locally
            _client = new DocumentClient(new Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            await _client.OpenAsync();
        }

        [SetUp]
        public async Task SetUp()
        {
            var database = _client.CreateDatabaseQuery().Where(d => d.Id == Data.DatabaseId).ToArray().FirstOrDefault();
            if (!object.ReferenceEquals(database, default(Database)))
            {
                await _client.DeleteDatabaseAsync(database.SelfLink);
            }

            _repository = new CosmosDbEventStore(_client, new CosmosDbQueryPager(), Data.DatabaseId, Data.Collectionid);
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
        public async Task When_saveing_a_document()
        {
            var @event = new ChangedAValue(Data.AValue) {AggregateRootId = Data.AggregateRootId1};
            await _repository.Save(Data.AggregateRootId1, new [] { @event });

            var result = _client.CreateDocumentQuery<EventPayloadWrapper>(
                UriFactory.CreateDocumentCollectionUri(Data.DatabaseId, Data.Collectionid), new FeedOptions()
                {
                    PartitionKey = new PartitionKey(Data.AggregateRootId1.ToString()),
                    EnableCrossPartitionQuery = false,
                    MaxDegreeOfParallelism = -1,
                    MaxBufferedItemCount = -1
                }).Where(e => e.Id == @event.Id).ToArray().FirstOrDefault();

            result.GetEvent().Should().BeEquivalentTo(@event);

        }

        [Test]
        public async Task When_getting_a_single_document()
        {
            var @event = new ChangedAValue(Data.AValue) { AggregateRootId = Data.AggregateRootId1 };
            await _repository.Save(Data.AggregateRootId1, new[] { @event });

            var result = (await _repository.Get(Data.AggregateRootId1)).ToArray();

            result.Should().HaveCount(1);
            result[0].Should().BeEquivalentTo(@event);
        }

        [Test]
        public async Task When_getting_multiple_documents()
        {
            var correctEvents = new IEvent[]
            {
                new ChangedAValue(Data.AValue) {AggregateRootId = Data.AggregateRootId1, Version = 3},
                new ChangedAValue(Data.AValue) {AggregateRootId = Data.AggregateRootId1, Version = 1},
                new ChangedAValue(Data.AValue) {AggregateRootId = Data.AggregateRootId1, Version = 2},
                new ChangedAValue(Data.AValue) {AggregateRootId = Data.AggregateRootId1, Version = 0},
            };
            var otherEvents = new IEvent[]
            {
                new ChangedAValue(Data.AValue) {AggregateRootId = Data.AggregateRootId2, Version = 3},
                new ChangedAValue(Data.AValue) {AggregateRootId = Data.AggregateRootId2, Version = 1},
                new ChangedAValue(Data.AValue) {AggregateRootId = Data.AggregateRootId2, Version = 2},
                new ChangedAValue(Data.AValue) {AggregateRootId = Data.AggregateRootId2, Version = 0},
            };
            await Task.WhenAll(_repository.Save(Data.AggregateRootId1, correctEvents), _repository.Save(Data.AggregateRootId2, otherEvents));
            var result = (await _repository.Get(Data.AggregateRootId1)).ToArray();

            result.Should().BeInAscendingOrder(e => e.Version);
            result.Should().OnlyContain(e => e.AggregateRootId == Data.AggregateRootId1);
        }


        private static class Data
        {
            public const string DatabaseId = "tempsoft_cqrs_tests";
            public const string Collectionid = "events";
            public const string DatabaseLink = "database";
            public const int AValue = 5;

            public static readonly Guid AggregateRootId1 = Guid.NewGuid();
            public static readonly Guid AggregateRootId2 = Guid.NewGuid();
        }
    }
}