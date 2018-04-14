using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.CosmosDb.Queries;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.CosmosDb.Queries
{
    [TestFixture]
    public class When_storing_a_model
    {
        private IDocumentClient _client;
        private CosmosDbQueryModelRepository _repository;
        private Database _database;
        private ICosmosDbQueryPager _pager;
        private AThingQueryModel _model;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _model = new AThingQueryModel() {A = Data.AValue, B = Data.BValue};
            _client = A.Fake<IDocumentClient>();
            _pager = A.Fake<ICosmosDbQueryPager>();
            _database = new Database(){ Id = Data.DatabaseId };
            A.CallTo(() => _client.CreateDatabaseQuery(A<FeedOptions>.Ignored)).Returns(Enumerable.Empty<Database>().AsQueryable().OrderBy(db => db.Id));
            A.CallTo(() => _client.CreateDatabaseAsync(A<Database>.Ignored, A<RequestOptions>.Ignored))
                .Returns(new ResourceResponse<Database>(_database));
            A.CallTo(() => _client.ReadDocumentCollectionFeedAsync(A<string>.Ignored, A<FeedOptions>.Ignored))
                .Returns(new FeedResponse<DocumentCollection>(Enumerable.Empty<DocumentCollection>()));

            _repository = new CosmosDbQueryModelRepository(_client, _pager, Data.DatabaseId, Data.Collectionid);

            await _repository.Save(Data.QueryModelId, _model, CancellationToken.None);
        }

        [Test]
        public void Should_have_upserted_the_document()
        {
            A.CallTo(() => _client.UpsertDocumentAsync(A<Uri>.That.Matches(uri => uri == UriFactory.CreateDocumentCollectionUri(Data.DatabaseId, Data.Collectionid)), A<object>.That.Matches(o => ((QueryModelPayloadWrapper)o).Id == Data.QueryModelId), A<RequestOptions>.Ignored, A<bool>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
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