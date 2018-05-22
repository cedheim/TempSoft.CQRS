﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using NUnit.Framework;
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Tests.CosmosDb.Events.EventStreamState
{
    [TestFixture]
    public class When_setting_the_status_of_en_event_stream
    {
        private IDocumentClient _client;
        private CosmosDbEventStreamStateManager _repository;
        private Database _database;
        private ICosmosDbQueryPager _pager;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _client = A.Fake<IDocumentClient>();
            _pager = A.Fake<ICosmosDbQueryPager>();
            _database = new Database(){ Id = Data.DatabaseId };

            A.CallTo(() => _client.CreateDatabaseQuery(A<FeedOptions>.Ignored)).Returns(Enumerable.Empty<Database>().AsQueryable().OrderBy(db => db.Id));
            A.CallTo(() => _client.CreateDatabaseAsync(A<Database>.Ignored, A<RequestOptions>.Ignored))
                .Returns(new ResourceResponse<Database>(_database));
            A.CallTo(() => _client.ReadDocumentCollectionFeedAsync(A<string>.Ignored, A<FeedOptions>.Ignored))
                .Returns(new FeedResponse<DocumentCollection>(Enumerable.Empty<DocumentCollection>()));
            A.CallTo(() => _client.ReadDocumentAsync(A<Uri>.Ignored, A<RequestOptions>.Ignored))
                .Throws(foc => new DocumentQueryException(string.Empty));

            _repository = new CosmosDbEventStreamStateManager(_client, _pager, Data.DatabaseId, Data.Collectionid);
            await _repository.SetStatusForStream(Data.EventStreamName, EventStreamStatus.Initialized);
        }

        [Test]
        public void Should_have_upserted_a_document()
        {
            A.CallTo(() => _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(Data.DatabaseId, Data.Collectionid), A<object>.That.Matches(o => ((CQRS.CosmosDb.Events.EventStreamState)o).Id == Data.EventStreamName && ((CQRS.CosmosDb.Events.EventStreamState)o).Status == EventStreamStatus.Initialized), A<RequestOptions>.Ignored, A<bool>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private static class Data
        {
            public const string DatabaseId = "tempsoft";
            public const string Collectionid = "event_stream_states";
            public const string DatabaseLink = "database";

            public const string EventStreamName = "EVENT STREAM";
        }
    }
}