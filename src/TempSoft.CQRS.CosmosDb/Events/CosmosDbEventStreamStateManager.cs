using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.CosmosDb.Queries;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.CosmosDb.Events
{
    public class CosmosDbEventStreamStateManager : RepositoryBase, IEventStreamStateManager
    {
        public CosmosDbEventStreamStateManager(IDocumentClient client, ICosmosDbQueryPager pager, string databaseId, string collectionId, int initialThroughput = 1000)
            : base(client, pager, databaseId, collectionId, initialThroughput)
        {
        }

        public override IEnumerable<string> PartitionKeyPaths => new string[0];

        public async Task<EventStreamStatus> GetStatusForStream(string streamName)
        {
            var state = await Get(streamName);
            return state.Status;
        }

        public async Task SetStatusForStream(string streamName, EventStreamStatus status)
        {
            var state = await Get(streamName);
            state.Status = status;
            await Save(state);
        }

        public async Task<int> GetEventCountForStream(string streamName)
        {
            var state = await Get(streamName);
            return state.EventCount;
        }

        public async Task AddToEventCountForStream(string streamName, int count = 1)
        {
            var state = await Get(streamName);
            state.EventCount += count;
            await Save(state);
        }

        private async Task<EventStreamState> Get(string streamName)
        {
            try
            {
                var uri = UriFactory.CreateDocumentUri(DatabaseId, CollectionId, streamName);

                var resource = await Client.ReadDocumentAsync(uri);
                return (EventStreamState) resource.Resource;
            }
            catch (DocumentClientException)
            {
                return new EventStreamState {Id = streamName};
            }
        }

        private async Task Save(EventStreamState state)
        {
            await Client.UpsertDocumentAsync(Uri, state);
        }

    }
}