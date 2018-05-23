using System.Linq;
using FakeItEasy;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using TempSoft.CQRS.CosmosDb.Commands;
using TempSoft.CQRS.CosmosDb.Infrastructure;

namespace TempSoft.CQRS.Tests.CosmosDb.Commands.CommandRegistry
{
    [TestFixture]
    public class When_creating_a_command_registry
    {
        private IDocumentClient _client;
        private CosmosDbCommandRegistry _repository;
        private Database _database;
        private ICosmosDbQueryPager _pager;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _client = A.Fake<IDocumentClient>();
            _pager = A.Fake<ICosmosDbQueryPager>();
            _database = new Database {Id = Data.DatabaseId};
            A.CallTo(() => _client.CreateDatabaseQuery(A<FeedOptions>.Ignored))
                .Returns(Enumerable.Empty<Database>().AsQueryable().OrderBy(db => db.Id));
            A.CallTo(() => _client.CreateDatabaseAsync(A<Database>.Ignored, A<RequestOptions>.Ignored))
                .Returns(new ResourceResponse<Database>(_database));
            A.CallTo(() => _client.ReadDocumentCollectionFeedAsync(A<string>.Ignored, A<FeedOptions>.Ignored))
                .Returns(new FeedResponse<DocumentCollection>(Enumerable.Empty<DocumentCollection>()));

            _repository = new CosmosDbCommandRegistry(_client, _pager, Data.DatabaseId, Data.Collectionid);
        }

        private static class Data
        {
            public const string DatabaseId = "tempsoft";
            public const string Collectionid = "commands";
            public const string DatabaseLink = "database";
        }

        [Test]
        public void Should_have_created_the_collection()
        {
            A.CallTo(() => _client.CreateDocumentCollectionAsync(_database.SelfLink,
                    A<DocumentCollection>.That.Matches(col => col.Id == Data.Collectionid),
                    A<RequestOptions>.That.Matches(opt => opt.OfferThroughput == 1000)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_created_the_database()
        {
            A.CallTo(() => _client.CreateDatabaseAsync(A<Database>.That.Matches(db => db.Id == Data.DatabaseId),
                    A<RequestOptions>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_listed_all_available_databases()
        {
            A.CallTo(() => _client.CreateDatabaseQuery(A<FeedOptions>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_read_the_collections()
        {
            A.CallTo(() => _client.ReadDocumentCollectionFeedAsync(_database.SelfLink, A<FeedOptions>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}