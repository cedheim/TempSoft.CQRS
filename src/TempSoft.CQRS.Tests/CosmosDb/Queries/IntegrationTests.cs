using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.CosmosDb.Queries;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.CosmosDb.Queries
{
    [TestFixture]
    [Category("Integration")]
    public class IntegrationTests
    {
        [SetUp]
        public async Task SetUp()
        {
            var database = _client.CreateDatabaseQuery().Where(d => d.Id == Data.DatabaseId).ToArray().FirstOrDefault();
            if (!ReferenceEquals(database, default(Database))) await _client.DeleteDatabaseAsync(database.SelfLink);

            _repository =
                new CosmosDbQueryModelRepository(_client, new CosmosDbQueryPager(), Data.DatabaseId, Data.Collectionid);
        }

        private CosmosDbQueryModelRepository _repository;
        private DocumentClient _client;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            // Connect to the Azure Cosmos DB Emulator running locally
            _client = new DocumentClient(new Uri("https://localhost:8081"),
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            await _client.OpenAsync();
        }


        private static class Data
        {
            public const string DatabaseId = "tempsoft_cqrs_tests";
            public const string Collectionid = "queries";
            public const int AValue1 = 5;
            public const string BValue1 = "WOOO";
            public const int AValue2 = 1;
            public const string BValue2 = "AAAW";

            public const string QuerModelId1 = "QUERYMODEL1";
            public const string QuerModelId2 = "QUERYMODEL2";
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
        public async Task When_getting_a_single_document()
        {
            var model = new AThingQueryModel {A = Data.AValue1, B = Data.BValue1};
            await _client.UpsertDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(Data.DatabaseId, Data.Collectionid),
                new QueryModelPayloadWrapper(Data.QuerModelId1, model));

            var result = await _repository.Get<AThingQueryModel>(Data.QuerModelId1, CancellationToken.None);
            result.Should().BeEquivalentTo(model);
        }

        [Test]
        public async Task When_getting_multiple_documents()
        {
            var model1 = new AThingQueryModel {A = Data.AValue1, B = Data.BValue1};
            await _client.UpsertDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(Data.DatabaseId, Data.Collectionid),
                new QueryModelPayloadWrapper(Data.QuerModelId1, model1));

            var model2 = new AThingQueryModel {A = Data.AValue2, B = Data.BValue2};
            await _client.UpsertDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(Data.DatabaseId, Data.Collectionid),
                new QueryModelPayloadWrapper(Data.QuerModelId2, model2));


            var result1 = await _repository.Get<AThingQueryModel>(Data.QuerModelId1, CancellationToken.None);
            var result2 = await _repository.Get<AThingQueryModel>(Data.QuerModelId2, CancellationToken.None);

            result1.Should().BeEquivalentTo(model1);
            result2.Should().BeEquivalentTo(model2);
        }

        [Test]
        public async Task When_saveing_a_document()
        {
            var model = new AThingQueryModel {A = Data.AValue1, B = Data.BValue1};
            await _repository.Save(Data.QuerModelId1, model, CancellationToken.None);
            var response =
                await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(Data.DatabaseId, Data.Collectionid,
                    Data.QuerModelId1));
            var wrapper = (QueryModelPayloadWrapper) response.Resource;
            var result = wrapper.GetPayload<AThingQueryModel>();

            result.Should().BeEquivalentTo(model);
        }
    }
}