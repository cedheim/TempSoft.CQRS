using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using TempSoft.CQRS.CosmosDb.Extensions;
using TempSoft.CQRS.Extensions;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.CosmosDb.Events
{
    public class CosmosDbEventStore : RepositoryBase, IEventStore
    {
        public CosmosDbEventStore(IDocumentClient client, ICosmosDbQueryPager pager, string databaseId,
            string collectionId, int initialThroughput = 1000)
            : base(client, pager, databaseId, collectionId, initialThroughput)
        {
        }

        public override IEnumerable<string> PartitionKeyPaths => new[] {"/PartitionId"};


        public async Task<IEnumerable<IEvent>> Get(string aggreagateRootId, int fromVersion = default(int),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(aggreagateRootId))
                throw new ArgumentNullException(nameof(aggreagateRootId));

            var query = Client.CreateDocumentQuery<EventPayloadWrapper>(Uri, new FeedOptions
                {
                    PartitionKey = new PartitionKey(aggreagateRootId),
                    EnableCrossPartitionQuery = false,
                    MaxDegreeOfParallelism = -1,
                    MaxBufferedItemCount = -1
                })
                .Where(wrapper => wrapper.DocumentType == EventPayloadWrapper.DocumentTypeName)
                .Where(wrapper => wrapper.PartitionId == aggreagateRootId)
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

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default(CancellationToken))
        {
            var wrappers = events.Select(e => new EventPayloadWrapper(e)).ToArray();
            var tasks = wrappers.Select(wrapper => Client.UpsertDocumentAsync(Uri, wrapper, new RequestOptions {PartitionKey = new PartitionKey(wrapper.PartitionId)}));
            await Task.WhenAll(tasks);
        }

        public async Task List(EventStoreFilter filter, Func<IEvent, CancellationToken, Task> callback,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            FeedOptions feedOptions;
            if (!string.IsNullOrEmpty(filter.AggregateRootId))
                feedOptions = new FeedOptions
                {
                    PartitionKey = new PartitionKey(filter.AggregateRootId),
                    EnableCrossPartitionQuery = false
                };
            else
                feedOptions = new FeedOptions
                {
                    EnableCrossPartitionQuery = true,
                    MaxDegreeOfParallelism = -1
                };

            IQueryable<EventPayloadWrapper> query = Client.CreateDocumentQuery<EventPayloadWrapper>(Uri, feedOptions);

            query = query.Where(e => e.DocumentType == EventPayloadWrapper.DocumentTypeName);

            if (!string.IsNullOrEmpty(filter.AggregateRootId))
                query = query.Where(e => e.PartitionId == filter.AggregateRootId);

            var eventTypes = filter.EventTypes?.Select(t => t.ToFriendlyName()).ToArray() ?? new string[0];

            if (eventTypes.Length > 0) query = query.Where(e => eventTypes.Contains(e.PayloadType));

            if (filter.EventGroups?.Length > 0) query = query.Where(e => filter.EventGroups.Contains(e.EventGroup));

            if (filter.From.HasValue)
            {
                var unixTime = filter.From.Value.ToUnixTime();
                query = query.Where(e => e.Epoch >= unixTime);
            }

            var pagedQuery = Pager.CreatePagedQuery(query.OrderBy(e => e.Epoch));

            while (pagedQuery.HasMoreResults && !cancellationToken.IsCancellationRequested)
            {
                var next =
                    (await pagedQuery.ExecuteNextAsync<EventPayloadWrapper>(cancellationToken)).OrderBy(w => w.Version);

                foreach (var wrapper in next) await callback(wrapper.GetEvent(), cancellationToken);
            }
        }
    }
}