﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.CosmosDb.Tests.Mocks;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Mocks;

namespace TempSoft.CQRS.CosmosDb.Tests.Events.EventStore
{
    [TestFixture]
    public class When_listing_all_events_for_specific_aggregate_root
    {
        private IDocumentClient _client;
        private CosmosDbEventStore _repository;
        private Database _database;
        private List<IEvent> _events;
        private IEvent _event1;
        private ICosmosDbQueryPager _pager;
        private ChangedAValue _event2;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _event1 = new ChangedAValue(Data.AValue) {AggregateRootId = Data.AggregateRootId1};
            _event2 = new ChangedAValue(Data.AValue) {AggregateRootId = Data.AggregateRootId2};
            _events = new List<IEvent>();

            _client = A.Fake<IDocumentClient>();
            _pager = A.Fake<ICosmosDbQueryPager>();
            _database = new Database {Id = Data.DatabaseId};
            A.CallTo(() => _client.CreateDatabaseQuery(A<FeedOptions>.Ignored))
                .Returns(Enumerable.Empty<Database>().AsQueryable().OrderBy(db => db.Id));
            A.CallTo(() => _client.CreateDatabaseAsync(A<Database>.Ignored, A<RequestOptions>.Ignored))
                .Returns(new ResourceResponse<Database>(_database));
            A.CallTo(() => _client.ReadDocumentCollectionFeedAsync(A<string>.Ignored, A<FeedOptions>.Ignored))
                .Returns(new FeedResponse<DocumentCollection>(Enumerable.Empty<DocumentCollection>()));
            A.CallTo(() => _pager.CreatePagedQuery(A<IQueryable<EventPayloadWrapper>>.Ignored))
                .ReturnsLazily(foc =>
                    new MockDocumentQuery<EventPayloadWrapper>(foc.GetArgument<IQueryable<EventPayloadWrapper>>(0)));
            A.CallTo(() => _client.CreateDocumentQuery<EventPayloadWrapper>(A<Uri>.Ignored, A<FeedOptions>.Ignored))
                .Returns(new[] {new EventPayloadWrapper(_event1), new EventPayloadWrapper(_event2)}.AsQueryable()
                    .OrderBy(e => e.Version));

            var filter = new EventStoreFilter
            {
                AggregateRootId = Data.AggregateRootId1
            };

            _repository = new CosmosDbEventStore(_client, _pager, Data.DatabaseId, Data.Collectionid);
            await _repository.List(filter, (e, c) => Task.Run(() => _events.Add(e), c));
        }

        private static class Data
        {
            public const string DatabaseId = "tempsoft";
            public const string Collectionid = "events";
            public const string DatabaseLink = "database";
            public const int AValue = 5;
            public const long Timestamp = 1525072880;
            public static readonly DateTime FilterTimestamp = new DateTime(2018, 04, 30, 12, 0, 0, DateTimeKind.Utc);


            public static readonly string AggregateRootId1 = Guid.NewGuid().ToString();
            public static readonly string AggregateRootId2 = Guid.NewGuid().ToString();
        }

        [Test]
        public void Should_have_called_the_client()
        {
            A.CallTo(() => _client.CreateDocumentQuery<EventPayloadWrapper>(
                    A<Uri>.That.Matches(e =>
                        e == UriFactory.CreateDocumentCollectionUri(Data.DatabaseId, Data.Collectionid)),
                    A<FeedOptions>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_returned_the_correct_event()
        {
            _events.Should().HaveCount(1);
            var @event = _events.FirstOrDefault();

            @event.Should().BeEquivalentTo(_event1);
        }

        [Test]
        public void Should_have_used_a_parition_key()
        {
            var pk = new PartitionKey(Data.AggregateRootId1.ToString());
            A.CallTo(() => _client.CreateDocumentQuery<EventPayloadWrapper>(A<Uri>.Ignored,
                    A<FeedOptions>.That.Matches(fo => fo.PartitionKey.Equals(pk) && !fo.EnableCrossPartitionQuery)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}