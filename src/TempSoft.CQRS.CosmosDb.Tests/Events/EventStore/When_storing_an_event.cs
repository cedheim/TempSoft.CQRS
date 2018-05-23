using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Mocks;

namespace TempSoft.CQRS.CosmosDb.Tests.Events.EventStore
{
    [TestFixture]
    public class When_storing_an_event
    {
        private IDocumentClient _client;
        private CosmosDbEventStore _repository;
        private Database _database;
        private ICosmosDbQueryPager _pager;
        private ChangedAValue _event;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _event = new ChangedAValue(Data.AValue) {AggregateRootId = Data.AggregateRootId};
            _client = A.Fake<IDocumentClient>();
            _pager = A.Fake<ICosmosDbQueryPager>();
            _database = new Database {Id = Data.DatabaseId};
            A.CallTo(() => _client.CreateDatabaseQuery(A<FeedOptions>.Ignored))
                .Returns(Enumerable.Empty<Database>().AsQueryable().OrderBy(db => db.Id));
            A.CallTo(() => _client.CreateDatabaseAsync(A<Database>.Ignored, A<RequestOptions>.Ignored))
                .Returns(new ResourceResponse<Database>(_database));
            A.CallTo(() => _client.ReadDocumentCollectionFeedAsync(A<string>.Ignored, A<FeedOptions>.Ignored))
                .Returns(new FeedResponse<DocumentCollection>(Enumerable.Empty<DocumentCollection>()));

            _repository = new CosmosDbEventStore(_client, _pager, Data.DatabaseId, Data.Collectionid);
            await _repository.Save(new[] {_event});
        }

        private static class Data
        {
            public const string DatabaseId = "tempsoft";
            public const string Collectionid = "events";
            public const string DatabaseLink = "database";
            public const int AValue = 5;

            public static readonly Guid AggregateRootId = Guid.NewGuid();
        }

        [Test]
        public void Should_have_upserted_the_event()
        {
            A.CallTo(() => _client.UpsertDocumentAsync(
                    A<Uri>.That.Matches(u =>
                        u == UriFactory.CreateDocumentCollectionUri(Data.DatabaseId, Data.Collectionid)),
                    A<object>.That.Matches(o => ((EventPayloadWrapper) o).Id == _event.Id), A<RequestOptions>.Ignored,
                    A<bool>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}