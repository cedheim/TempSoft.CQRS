using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using TempSoft.CQRS.Common.Extensions;
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

        public override IEnumerable<string> PartitionKeyPaths => new[] {"/AggregateRootId"};


        public async Task<IEnumerable<IEvent>> Get(Guid id, int fromVersion = default(int),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = Client.CreateDocumentQuery<EventPayloadWrapper>(Uri, new FeedOptions
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

        public async Task Save(IEnumerable<IEvent> events,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var wrappers = events.Select(e => new EventPayloadWrapper(e)).ToArray();
            var tasks = wrappers.Select(wrapper => Client.UpsertDocumentAsync(Uri, wrapper,
                new RequestOptions {PartitionKey = new PartitionKey(wrapper.AggregateRootId.ToString())}));
            await Task.WhenAll(tasks);
        }

        public async Task List(EventStoreFilter filter, Func<IEvent, CancellationToken, Task> callback,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            FeedOptions feedOptions;
            if (filter.AggregateRootId.HasValue)
                feedOptions = new FeedOptions
                {
                    PartitionKey = new PartitionKey(filter.AggregateRootId.Value.ToString()),
                    EnableCrossPartitionQuery = false
                };
            else
                feedOptions = new FeedOptions
                {
                    EnableCrossPartitionQuery = true,
                    MaxDegreeOfParallelism = -1
                };

            IQueryable<EventPayloadWrapper> query = Client.CreateDocumentQuery<EventPayloadWrapper>(Uri, feedOptions);

            if (filter.AggregateRootId.HasValue)
                query = query.Where(e => e.AggregateRootId == filter.AggregateRootId.Value);

            var eventTypes = filter.EventTypes?.Select(t => t.ToFriendlyName()).ToArray() ?? new string[0];

            if (eventTypes.Length > 0) query = query.Where(e => eventTypes.Contains(e.PayloadType));

            if (filter.EventGroups?.Length > 0) query = query.Where(e => filter.EventGroups.Contains(e.EventGroup));

            if (filter.From.HasValue)
            {
                var unixTime = filter.From.Value.ToUnixTime();
                query = query.Where(e => e.Timestamp >= unixTime);
            }

            var pagedQuery = Pager.CreatePagedQuery(query.OrderBy(e => e.Timestamp));

            while (pagedQuery.HasMoreResults && !cancellationToken.IsCancellationRequested)
            {
                var next =
                    (await pagedQuery.ExecuteNextAsync<EventPayloadWrapper>(cancellationToken)).OrderBy(w => w.Version);

                foreach (var wrapper in next) await callback(wrapper.GetEvent(), cancellationToken);
            }
        }
    }
}