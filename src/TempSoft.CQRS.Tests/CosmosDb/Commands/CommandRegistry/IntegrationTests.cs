using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using TempSoft.CQRS.CosmosDb.Commands;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Tests.CosmosDb.Commands.CommandRegistry
{
    [TestFixture]
    [Category("Integration")]
    public class IntegrationTests
    {
        private CosmosDbCommandRegistry _repository;
        private DocumentClient _client;
        private IEvent[] _events;
        private Uri _uri;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            // Connect to the Azure Cosmos DB Emulator running locally
            _client = new DocumentClient(new Uri("https://localhost:8081"),
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            await _client.OpenAsync();

            var database = _client.CreateDatabaseQuery().Where(d => d.Id == Data.DatabaseId).ToArray().FirstOrDefault();
            if (!ReferenceEquals(database, default(Database))) await _client.DeleteDatabaseAsync(database.SelfLink);

            _repository =
                new CosmosDbCommandRegistry(_client, new CosmosDbQueryPager(), Data.DatabaseId, Data.Collectionid);
            await _repository.Save(Data.AggregateRootId1, Data.CommandIds1);
            await _repository.Save(Data.AggregateRootId2, Data.CommandIds2);
        }

        private static class Data
        {
            public const string DatabaseId = "tempsoft_cqrs_tests";
            public const string Collectionid = "commands";


            public static readonly Guid AggregateRootId1 = Guid.NewGuid();
            public static readonly Guid AggregateRootId2 = Guid.NewGuid();
            public static readonly Guid AggregateRootId3 = Guid.NewGuid();

            public static readonly Guid[] CommandIds1 = {Guid.NewGuid()};

            public static readonly Guid[] CommandIds2 =
                {Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};
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
        public async Task When_getting_command_ids_for_an_aggregate_root_without_any_command_ids()
        {
            var commandIds = (await _repository.Get(Data.AggregateRootId3, CancellationToken.None)).ToArray();
            commandIds.Should().HaveCount(0);
        }

        [Test]
        public async Task When_getting_multiple_command_ids()
        {
            var commandIds = (await _repository.Get(Data.AggregateRootId2, CancellationToken.None)).ToArray();
            commandIds.Should().HaveCount(Data.CommandIds2.Length);
            commandIds.Should().OnlyContain(id => Data.CommandIds2.Contains(id));
        }

        [Test]
        public async Task When_getting_single_command_ids()
        {
            var commandIds = (await _repository.Get(Data.AggregateRootId1, CancellationToken.None)).ToArray();
            commandIds.Should().HaveCount(Data.CommandIds1.Length);
            commandIds.Should().OnlyContain(id => Data.CommandIds1.Contains(id));
        }
    }
}