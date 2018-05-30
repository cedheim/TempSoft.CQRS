using System;
using System.Collections.Generic;
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
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.CosmosDb.Projectors;
using TempSoft.CQRS.CosmosDb.Tests.Mocks;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.CosmosDb.Tests.Projectors
{
    [TestFixture]
    public class When_listing_projections_from_the_repository
    {
        private IDocumentClient _client;
        private CosmosDbProjectionModelRepository _repository;
        private Database _database;
        private List<IProjection> _result;
        private IProjection[] _projections;
        private ICosmosDbQueryPager _pager;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _projections = new IProjection[]
            {
                new AThingProjection(Data.ProjectionId1, Data.ProjectorId1)
                {
                    A = Data.AValue,
                    B = Data.BValue
                },
                new AThingProjection(Data.ProjectionId2, Data.ProjectorId1)
                {
                    A = Data.AValue,
                    B = Data.BValue
                },
                new AThingProjection(Data.ProjectionId3, Data.ProjectorId2)
                {
                    A = Data.AValue,
                    B = Data.BValue
                }
            };

            _result = new List<IProjection>();
            _client = A.Fake<IDocumentClient>();
            _pager = A.Fake<ICosmosDbQueryPager>();
            _database = new Database { Id = Data.DatabaseId };
            A.CallTo(() => _client.CreateDatabaseQuery(A<FeedOptions>.Ignored))
                .Returns(Enumerable.Empty<Database>().AsQueryable().OrderBy(db => db.Id));
            A.CallTo(() => _client.CreateDatabaseAsync(A<Database>.Ignored, A<RequestOptions>.Ignored))
                .Returns(new ResourceResponse<Database>(_database));
            A.CallTo(() => _client.ReadDocumentCollectionFeedAsync(A<string>.Ignored, A<FeedOptions>.Ignored))
                .Returns(new FeedResponse<DocumentCollection>(Enumerable.Empty<DocumentCollection>()));
            A.CallTo(() => _pager.CreatePagedQuery(A<IQueryable<ProjectionPayloadWrapper>>.Ignored))
                .ReturnsLazily(foc => new MockDocumentQuery<ProjectionPayloadWrapper>(foc.GetArgument<IQueryable<ProjectionPayloadWrapper>>(0)));
            A.CallTo(() => _client.CreateDocumentQuery<ProjectionPayloadWrapper>(A<Uri>.Ignored, A<FeedOptions>.Ignored))
                .Returns(_projections.Select(e => new ProjectionPayloadWrapper(e)).AsQueryable().OrderBy(e => e.Id));

            _repository = new CosmosDbProjectionModelRepository(_client, _pager, Data.DatabaseId, Data.Collectionid);
            await _repository.List(Data.ProjectorId1, (projection, token) => Task.Run(() => _result.Add(projection)), CancellationToken.None);
        }

        [Test]
        public void Should_have_called_the_client()
        {
            A.CallTo(() => _client.CreateDocumentQuery<ProjectionPayloadWrapper>(A<Uri>.That.Matches(e => e == UriFactory.CreateDocumentCollectionUri(Data.DatabaseId, Data.Collectionid)), A<FeedOptions>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_returned_the_correct_event()
        {
            _result.Count.Should().Be(2);
            _result.Should().OnlyContain(r => r.ProjectorId == Data.ProjectorId1);
        }

        private static class Data
        {
            public const string ProjectionId1 = "Projection1";
            public const string ProjectionId2 = "Projection2";
            public const string ProjectionId3 = "Projection3";
            public const string ProjectorId1 = "Projector1";
            public const string ProjectorId2 = "Projector2";

            public const string DatabaseId = "tempsoft";
            public const string Collectionid = "events";
            public const string DatabaseLink = "database";
            public const int AValue = 5;
            public const string BValue = "HELLO WORLD?";

            public static readonly Guid AggregateRootId = Guid.NewGuid();
        }
    }
}