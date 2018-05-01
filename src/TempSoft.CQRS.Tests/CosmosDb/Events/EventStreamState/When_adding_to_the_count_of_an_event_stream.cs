﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using NUnit.Framework;
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.CosmosDb.Queries;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Tests.CosmosDb.Events.EventStreamState
{
    [TestFixture]
    public class When_adding_to_the_count_of_an_event_stream
    {
        private IDocumentClient _client;
        private CosmosDbEventStreamStateManager _repository;
        private Database _database;
        private ICosmosDbQueryPager _pager;
        private CQRS.CosmosDb.Events.EventStreamState _state;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _client = A.Fake<IDocumentClient>();
            _pager = A.Fake<ICosmosDbQueryPager>();
            _database = new Database(){ Id = Data.DatabaseId };

            _state = new CQRS.CosmosDb.Events.EventStreamState
            {
                EventCount = 5,
                Id = Data.EventStreamName,
                Status = EventStreamStatus.Initialized
            };

            var document = new Document();
            document.LoadFrom(new JsonTextReader(new StringReader(Newtonsoft.Json.JsonConvert.SerializeObject(_state))));
            
            A.CallTo(() => _client.CreateDatabaseQuery(A<FeedOptions>.Ignored)).Returns(Enumerable.Empty<Database>().AsQueryable().OrderBy(db => db.Id));
            A.CallTo(() => _client.CreateDatabaseAsync(A<Database>.Ignored, A<RequestOptions>.Ignored))
                .Returns(new ResourceResponse<Database>(_database));
            A.CallTo(() => _client.ReadDocumentCollectionFeedAsync(A<string>.Ignored, A<FeedOptions>.Ignored))
                .Returns(new FeedResponse<DocumentCollection>(Enumerable.Empty<DocumentCollection>()));
            A.CallTo(() => _client.ReadDocumentAsync(A<Uri>.Ignored, A<RequestOptions>.Ignored))
                .Returns(new ResourceResponse<Document>(document));

            _repository = new CosmosDbEventStreamStateManager(_client, _pager, Data.DatabaseId, Data.Collectionid);
            await _repository.AddToEventCountForStream(Data.EventStreamName);
        }

        [Test]
        public void Should_have_read_an_object()
        {
            var uri = UriFactory.CreateDocumentUri(Data.DatabaseId, Data.Collectionid, Data.EventStreamName);

            A.CallTo(() => _client.ReadDocumentAsync(uri, A<RequestOptions>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_upserted_a_document()
        {
            A.CallTo(() => _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(Data.DatabaseId, Data.Collectionid), A<object>.That.Matches(o => ((CQRS.CosmosDb.Events.EventStreamState)o).Id == Data.EventStreamName && ((CQRS.CosmosDb.Events.EventStreamState)o).EventCount == _state.EventCount + 1), A<RequestOptions>.Ignored, A<bool>.Ignored))
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