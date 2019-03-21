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
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.CosmosDb.Projectors;
using TempSoft.CQRS.CosmosDb.Tests.Mocks;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.CosmosDb.Tests.Projectors
{
    [TestFixture]
    public class When_getting_an_projection_from_the_repository
    {
        private IDocumentClient _client;
        private CosmosDbProjectionModelRepository _repository;
        private Database _database;
        private IProjection _result;
        private IProjection _projection;
        private ICosmosDbQueryPager _pager;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _projection = new AThingProjection(Data.ProjectionId, Data.ProjectorId)
            {
                A = Data.AValue,
                B = Data.BValue
            };

            var document = new Document();
            document.LoadFrom(new JsonTextReader(new StringReader(Newtonsoft.Json.JsonConvert.SerializeObject(new ProjectionPayloadWrapper(_projection)))));

            _client = A.Fake<IDocumentClient>();
            _pager = A.Fake<ICosmosDbQueryPager>();
            _database = new Database { Id = Data.DatabaseId };
            A.CallTo(() => _client.CreateDatabaseQuery(A<FeedOptions>.Ignored))
                .Returns(Enumerable.Empty<Database>().AsQueryable().OrderBy(db => db.Id));
            A.CallTo(() => _client.CreateDatabaseAsync(A<Database>.Ignored, A<RequestOptions>.Ignored))
                .Returns(new ResourceResponse<Database>(_database));
            A.CallTo(() => _client.ReadDocumentCollectionFeedAsync(A<string>.Ignored, A<FeedOptions>.Ignored))
                .Returns(new FeedResponse<DocumentCollection>(Enumerable.Empty<DocumentCollection>()));
            A.CallTo(() => _client.ReadDocumentAsync(A<Uri>.Ignored, A<RequestOptions>.Ignored))
                .Returns(new ResourceResponse<Document>(document));

            _repository = new CosmosDbProjectionModelRepository(_client, _pager, Data.DatabaseId, Data.Collectionid);
            _result = (await _repository.Get<AThingProjection>(Data.ProjectionId, nameof(AThingProjector), CancellationToken.None));
        }

        [Test]
        public void Should_have_called_the_client()
        {
            var documentId = ProjectionPayloadWrapper.CreateIdentifier(Data.ProjectionId);
            A.CallTo(() => _client.ReadDocumentAsync(A<Uri>.That.Matches(uri => uri == UriFactory.CreateDocumentUri(Data.DatabaseId, Data.Collectionid, documentId)), A<RequestOptions>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_returned_the_correct_event()
        {
            _result.Should().BeEquivalentTo(_projection);
        }

        private static class Data
        {
            public const string ProjectionId = "Projection1";
            public const string ProjectorId = "Projector1";

            public const string DatabaseId = "tempsoft";
            public const string Collectionid = "events";
            public const string DatabaseLink = "database";
            public const int AValue = 5;
            public const string BValue = "HELLO WORLD?";

            public static readonly Guid AggregateRootId = Guid.NewGuid();
        }
    }
}