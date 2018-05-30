using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using TempSoft.CQRS.Common.Extensions;
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.CosmosDb.Projectors;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.CosmosDb.Tests.Projectors
{
    [TestFixture]
    [Category("Integration")]
    public class IntegrationTests
    {
        private CosmosDbProjectionModelRepository _repository;
        private DocumentClient _client;
        private IProjection[] _projections;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            // Connect to the Azure Cosmos DB Emulator running locally
            _client = new DocumentClient(new Uri("https://localhost:8081"),
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            await _client.OpenAsync();

            var database = _client.CreateDatabaseQuery().Where(d => d.Id == Data.DatabaseId).ToArray().FirstOrDefault();
            if (!ReferenceEquals(database, default(Database))) await _client.DeleteDatabaseAsync(database.SelfLink);

            _repository = new CosmosDbProjectionModelRepository(_client, new CosmosDbQueryPager(), Data.DatabaseId, Data.Collectionid);

            _projections = new IProjection[]
            {
                new AThingProjection(Data.ProjectionId1, Data.ProjectorId1) { A = Data.AValue, B = Data.BValue },
                new AThingProjection(Data.ProjectionId2, Data.ProjectorId1) { A = Data.AValue, B = Data.BValue },
                new AThingProjection(Data.ProjectionId3, Data.ProjectorId1) { A = Data.AValue, B = Data.BValue },
                new AThingProjection(Data.ProjectionId4, Data.ProjectorId1) { A = Data.AValue, B = Data.BValue },
                new AThingProjection(Data.ProjectionId5, Data.ProjectorId2) { A = Data.AValue, B = Data.BValue },
                new AThingProjection(Data.ProjectionId6, Data.ProjectorId2) { A = Data.AValue, B = Data.BValue },
                new AThingProjection(Data.ProjectionId1, Data.ProjectorId2) { A = Data.AValue, B = Data.BValue },
            };

            await Task.WhenAll(_projections.Select(p => _repository.Save(p, CancellationToken.None)));
        }

        [Test]
        public async Task When_creating_the_repository()
        {
            var database = _client.CreateDatabaseQuery().Where(d => d.Id == Data.DatabaseId).ToArray().FirstOrDefault();
            database.Should().NotBeNull();

            var collections = await _client.ReadDocumentCollectionFeedAsync(database.SelfLink);
            var collection = collections.Where(coll => coll.Id == Data.Collectionid).ToArray().FirstOrDefault();

            collection.Should().NotBeNull();
        }

        [Test]
        public async Task When_getting_a_document()
        {
            var result = await _repository.Get<AThingProjection>(Data.ProjectionId1, Data.ProjectorId1, CancellationToken.None);
            result.Should().BeEquivalentTo(_projections[0]);
        }


        [Test]
        public async Task When_getting_a_document_that_does_not_exist()
        {
            var result = await _repository.Get<AThingProjection>(Guid.NewGuid().ToString(), Data.ProjectorId3, CancellationToken.None);
            result.Should().BeNull();
        }


        [Test]
        public async Task When_listing_documents()
        {
            var result = new List<IProjection>();
            await _repository.List(Data.ProjectorId1, (projection, token) => Task.Run(() => result.Add(projection)), CancellationToken.None);

            result.Should().HaveCount(_projections.Count(p => p.ProjectorId == Data.ProjectorId1));
        }

        [Test]
        public async Task When_getting_projections_with_same_id_in_different_partitions()
        {
            var result1 = await _repository.Get<AThingProjection>(Data.ProjectionId1, Data.ProjectorId1, CancellationToken.None);
            var result2 = await _repository.Get<AThingProjection>(Data.ProjectionId1, Data.ProjectorId2, CancellationToken.None);

            result1.Id.Should().Be(Data.ProjectionId1);
            result1.ProjectorId.Should().Be(Data.ProjectorId1);
            result2.Id.Should().Be(Data.ProjectionId1);
            result2.ProjectorId.Should().Be(Data.ProjectorId2);
        }

        private static class Data
        {
#if NETCOREAPP2_0
            public const string DatabaseId = "tempsoft_cqrs_tests_core";
#else
            public const string DatabaseId = "tempsoft_cqrs_tests_net452";
#endif
            public const string Collectionid = "projections";
            public const string DatabaseLink = "database";
            public const int AValue = 5;
            public const string BValue = "DOH";

            public static readonly string ProjectorId1 = Guid.NewGuid().ToString();
            public static readonly string ProjectorId2 = Guid.NewGuid().ToString();
            public static readonly string ProjectorId3 = Guid.NewGuid().ToString();

            public static readonly string ProjectionId1 = Guid.NewGuid().ToString();
            public static readonly string ProjectionId2 = Guid.NewGuid().ToString();
            public static readonly string ProjectionId3 = Guid.NewGuid().ToString();
            public static readonly string ProjectionId4 = Guid.NewGuid().ToString();
            public static readonly string ProjectionId5 = Guid.NewGuid().ToString();
            public static readonly string ProjectionId6 = Guid.NewGuid().ToString();
        }
    }
}