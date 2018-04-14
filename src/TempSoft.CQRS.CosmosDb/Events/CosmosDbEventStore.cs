using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using TempSoft.CQRS.CosmosDb.Excpetions;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.CosmosDb.Events
{
    public class CosmosDbEventStore : RepositoryBase, IEventStore
    {
        public CosmosDbEventStore(IDocumentClient client, ICosmosDbQueryPager pager, string databaseId, string collectionId, int initialThroughput = 1000)
            : base(client, pager, databaseId, collectionId, initialThroughput)
        {
        }


        public async Task<IEnumerable<IEvent>> Get(Guid id, int fromVersion = default(int), CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = Client.CreateDocumentQuery<EventPayloadWrapper>(Uri, new FeedOptions()
                {
                    PartitionKey = new PartitionKey(id.ToString()),
                    EnableCrossPartitionQuery = false,
                    MaxDegreeOfParallelism = -1,
                    MaxBufferedItemCount = -1
                })
                .Where(wrapper => wrapper.AggregateRootId == id)
                .Where(wrapper => wrapper.Version >= fromVersion);

            var pagedQuery = Pager.CreatePagedQuery(query);
            var events = new List<IEvent>();
            while (pagedQuery.HasMoreResults)
            {
                var page = await pagedQuery.ExecuteNextAsync<EventPayloadWrapper>(cancellationToken);
                events.AddRange(page.Select(e => e.GetEvent()));
            }

            return events.OrderBy(e => e.Version);
        }

        public async Task Save(Guid id, IEnumerable<IEvent> events, CancellationToken cancellationToken = default(CancellationToken))
        {
            var wrappers = events.Select(e => new EventPayloadWrapper(e)).ToArray();
            if (wrappers.Select(e => e.AggregateRootId).Any(i => i != id))
            {
                throw new MultipleAggregateRootException($"Can not save events for multiple different events.");
            }

            var requestOptions = new RequestOptions()
            {
                PartitionKey = new PartitionKey(id.ToString())
            };

            var tasks = wrappers.Select(wrapper => Client.UpsertDocumentAsync(Uri, wrapper, requestOptions));
            await Task.WhenAll(tasks);
        }

        public override IEnumerable<string> PartitionKeyPaths => new[] {"/AggregateRootId"};
    }
}