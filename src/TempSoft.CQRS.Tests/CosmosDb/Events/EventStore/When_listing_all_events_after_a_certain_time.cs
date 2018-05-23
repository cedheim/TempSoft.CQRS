using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using TempSoft.CQRS.Common.Extensions;
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.CosmosDb.Events.EventStore
{
    [TestFixture]
    public class When_listing_all_events_after_a_certain_time
    {
        private IDocumentClient _client;
        private CosmosDbEventStore _repository;
        private Database _database;
        private List<IEvent> _result;
        private IEvent _event1;
        private ICosmosDbQueryPager _pager;
        private ChangedAValue _event2;
        private IEvent[] _events;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _events = new IEvent[]
            {
                new ChangedAValue(Data.AValue)
                {
                    Version = 0,
                    AggregateRootId = Data.AggregateRootId1,
                    EventGroup = nameof(AThingAggregateRoot),
                    Timestamp = Data.Timestamp
                },
                new ChangedAValue(Data.AValue)
                {
                    Version = 0,
                    AggregateRootId = Data.AggregateRootId2,
                    EventGroup = nameof(AThingAggregateRoot),
                    Timestamp = Data.Timestamp.AddHours(1)
                },
                new ChangedAValue(Data.AValue)
                {
                    Version = 1,
                    AggregateRootId = Data.AggregateRootId1,
                    EventGroup = nameof(AThingAggregateRoot),
                    Timestamp = Data.Timestamp.AddHours(2)
                },
                new ChangedBValue(Data.BValue)
                {
                    Version = 1,
                    AggregateRootId = Data.AggregateRootId2,
                    EventGroup = nameof(AThingAggregateRoot),
                    Timestamp = Data.Timestamp.AddHours(3)
                },
                new ChangedBValue(Data.BValue)
                {
                    Version = 2,
                    AggregateRootId = Data.AggregateRootId1,
                    EventGroup = nameof(AThingAggregateRoot),
                    Timestamp = Data.Timestamp.AddHours(4)
                },
                new ChangedBValue(Data.BValue)
                {
                    Version = 2,
                    AggregateRootId = Data.AggregateRootId2,
                    EventGroup = nameof(AThingAggregateRoot),
                    Timestamp = Data.Timestamp.AddHours(5)
                }
            };
            _result = new List<IEvent>();

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
                .Returns(_events.Select(e => new EventPayloadWrapper(e) {Timestamp = e.Timestamp.ToUnixTime()})
                    .AsQueryable().OrderBy(e => e.Version));

            var filter = new EventStoreFilter
            {
                From = Data.Timestamp.AddHours(1.1)
            };

            _repository = new CosmosDbEventStore(_client, _pager, Data.DatabaseId, Data.Collectionid);
            await _repository.List(filter, (e, c) => Task.Run(() => _result.Add(e), c));
        }

        private static class Data
        {
            public const string DatabaseId = "tempsoft";
            public const string Collectionid = "events";
            public const string DatabaseLink = "database";
            public const int AValue = 5;
            public const string BValue = "DOH";
            public static readonly DateTime Timestamp = new DateTime(2018, 04, 30, 12, 0, 0, DateTimeKind.Utc);


            public static readonly Guid AggregateRootId1 = Guid.NewGuid();
            public static readonly Guid AggregateRootId2 = Guid.NewGuid();
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
            _result.Should().OnlyContain(e => e.Timestamp > Data.Timestamp.AddHours(1));
        }

        [Test]
        public void Should_not_have_used_a_parition_key()
        {
            var pk = new PartitionKey(Data.AggregateRootId1.ToString());
            A.CallTo(() => _client.CreateDocumentQuery<EventPayloadWrapper>(A<Uri>.Ignored,
                    A<FeedOptions>.That.Matches(fo =>
                        fo.PartitionKey == null && fo.EnableCrossPartitionQuery && fo.MaxDegreeOfParallelism == -1)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}