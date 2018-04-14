using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using NUnit.Framework;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.CosmosDb.Queries;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.CosmosDb.Queries
{
    [TestFixture]
    public class When_getting_a_model
    {
        private IDocumentClient _client;
        private CosmosDbQueryModelRepository _repository;
        private Database _database;
        private ICosmosDbQueryPager _pager;
        private AThingQueryModel _model;
        private AThingQueryModel _result;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _model = new AThingQueryModel() { A = Data.AValue, B = Data.BValue };
            _client = A.Fake<IDocumentClient>();
            _pager = A.Fake<ICosmosDbQueryPager>();
            _database = new Database(){ Id = Data.DatabaseId };

            var document = new Document();
            document.LoadFrom(new JsonTextReader(new StringReader(Newtonsoft.Json.JsonConvert.SerializeObject(new QueryModelPayloadWrapper(Data.QueryModelId, _model)))));

            A.CallTo(() => _client.CreateDatabaseQuery(A<FeedOptions>.Ignored)).Returns(Enumerable.Empty<Database>().AsQueryable().OrderBy(db => db.Id));
            A.CallTo(() => _client.CreateDatabaseAsync(A<Database>.Ignored, A<RequestOptions>.Ignored))
                .Returns(new ResourceResponse<Database>(_database));
            A.CallTo(() => _client.ReadDocumentCollectionFeedAsync(A<string>.Ignored, A<FeedOptions>.Ignored))
                .Returns(new FeedResponse<DocumentCollection>(Enumerable.Empty<DocumentCollection>()));
            A.CallTo(() => _client.ReadDocumentAsync(A<Uri>.Ignored, A<RequestOptions>.Ignored))
                .Returns(new ResourceResponse<Document>(document));

            _repository = new CosmosDbQueryModelRepository(_client, _pager, Data.DatabaseId, Data.Collectionid);
            _result = await _repository.Get<AThingQueryModel>(Data.QueryModelId, CancellationToken.None);
        }

        [Test]
        public void Should_have_read_the_document_from_the_database()
        {
            A.CallTo(() => _client.ReadDocumentAsync(A<Uri>.That.Matches(uri => uri == UriFactory.CreateDocumentUri(Data.DatabaseId, Data.Collectionid, Data.QueryModelId)), A<RequestOptions>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_returned_the_correct_result()
        {
            _result.Should().BeEquivalentTo(_model);
        }

        private static class Data
        {
            public const string DatabaseId = "tempsoft";
            public const string Collectionid = "queries";
            public const string DatabaseLink = "database";
            public const string QueryModelId = "QUERYMODEL1";

            public const int AValue = 5;
            public const string BValue = "WOO";
        }
    }
}