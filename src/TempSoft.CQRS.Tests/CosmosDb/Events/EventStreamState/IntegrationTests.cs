using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Tests.CosmosDb.Events.EventStreamState
{
    [TestFixture]
    [Category("Integration")]
    public class IntegrationTests
    {
        private DocumentClient _client;
        private CosmosDbEventStreamStateManager _repository;
        private CQRS.CosmosDb.Events.EventStreamState _state;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            // Connect to the Azure Cosmos DB Emulator running locally
            _client = new DocumentClient(new Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            await _client.OpenAsync();

            _state = new CQRS.CosmosDb.Events.EventStreamState() { EventCount = 5, Id = Data.StreamName1, Status = EventStreamStatus.Initializing };

            var database = _client.CreateDatabaseQuery().Where(d => d.Id == Data.DatabaseId).ToArray().FirstOrDefault();
            if (!object.ReferenceEquals(database, default(Database)))
            {
                await _client.DeleteDatabaseAsync(database.SelfLink);
            }

            _repository = new CosmosDbEventStreamStateManager(_client, new CosmosDbQueryPager(), Data.DatabaseId, Data.Collectionid);

            await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(Data.DatabaseId, Data.Collectionid), _state);
            
        }

        [Test]
        public async Task When_getting_an_event_stream_which_does_not_exist()
        {
            var status = await _repository.GetStatusForStream(Guid.NewGuid().ToString());
            status.Should().Be(EventStreamStatus.Uninitialized);
        }

        [Test]
        public async Task When_getting_the_status()
        {
            var status = await _repository.GetStatusForStream(Data.StreamName1);
            status.Should().Be(_state.Status);
        }

        [Test]
        public async Task When_getting_the_count()
        {
            var count = await _repository.GetEventCountForStream(Data.StreamName1);
            count.Should().Be(_state.EventCount);
        }

        [Test]
        public async Task When_adding_to_the_count()
        {
            await _repository.AddToEventCountForStream(Data.StreamName2, 17);

            var resource = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(Data.DatabaseId, Data.Collectionid, Data.StreamName2));
            var state = (CQRS.CosmosDb.Events.EventStreamState)resource.Resource;

            state.Id.Should().Be(Data.StreamName2);
            state.EventCount.Should().Be(17);
        }

        [Test]
        public async Task When_setting_the_status_of_en_event_stream()
        {
            await _repository.SetStatusForStream(Data.StreamName2, EventStreamStatus.Initialized);

            var resource = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(Data.DatabaseId, Data.Collectionid, Data.StreamName2));
            var state = (CQRS.CosmosDb.Events.EventStreamState)resource.Resource;

            state.Id.Should().Be(Data.StreamName2);
            state.Status.Should().Be(EventStreamStatus.Initialized);
        }

        private static class Data
        {
            public const string DatabaseId = "tempsoft_cqrs_tests";
            public const string Collectionid = "event_stream_states";
            public const string DatabaseLink = "database";

            public const string StreamName1 = "EVENT_STREAM_1";
            public const string StreamName2 = "EVENT_STREAM_2";
        }

    }


}