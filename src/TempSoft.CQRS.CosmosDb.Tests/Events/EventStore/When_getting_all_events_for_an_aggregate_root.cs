﻿using System;
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
    public class When_getting_all_events_for_an_aggregate_root
    {
        private IDocumentClient _client;
        private CosmosDbEventStore _repository;
        private Database _database;
        private IEvent[] _events;
        private IEvent _event;
        private ICosmosDbQueryPager _pager;

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
            A.CallTo(() => _pager.CreatePagedQuery(A<IQueryable<EventPayloadWrapper>>.Ignored))
                .ReturnsLazily(foc =>
                    new MockDocumentQuery<EventPayloadWrapper>(foc.GetArgument<IQueryable<EventPayloadWrapper>>(0)));
            A.CallTo(() => _client.CreateDocumentQuery<EventPayloadWrapper>(A<Uri>.Ignored, A<FeedOptions>.Ignored))
                .Returns(Enumerable.Repeat(new EventPayloadWrapper(_event), 1).AsQueryable().OrderBy(e => e.Version));

            _repository = new CosmosDbEventStore(_client, _pager, Data.DatabaseId, Data.Collectionid);
            _events = (await _repository.Get(Data.AggregateRootId)).ToArray();
        }

        private static class Data
        {
            public const string DatabaseId = "tempsoft";
            public const string Collectionid = "events";
            public const string DatabaseLink = "database";
            public const int AValue = 5;

            public static readonly string AggregateRootId = Guid.NewGuid().ToString();
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

            @event.Should().BeEquivalentTo(_event);
        }
    }
}