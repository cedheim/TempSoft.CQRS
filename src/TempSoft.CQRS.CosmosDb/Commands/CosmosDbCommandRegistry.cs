using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.CosmosDb.Infrastructure;

namespace TempSoft.CQRS.CosmosDb.Commands
{
    public class CosmosDbCommandRegistry : RepositoryBase, ICommandRegistry
    {
        public CosmosDbCommandRegistry(IDocumentClient client, ICosmosDbQueryPager pager, string databaseId,
            string collectionId, int initialThroughput = 1000)
            : base(client, pager, databaseId, collectionId, initialThroughput)
        {
        }

        public override IEnumerable<string> PartitionKeyPaths => new[] { "/PartitionId" };

        public async Task<IEnumerable<Guid>> Get(string aggregateRootId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(aggregateRootId))
                throw new ArgumentNullException(nameof(aggregateRootId));

            var list = new List<Guid>();
            var query = Client.CreateDocumentQuery<CommandRegistryWrapper>(Uri,
                new FeedOptions
                {
                    PartitionKey = new PartitionKey(aggregateRootId),
                    MaxDegreeOfParallelism = -1,
                    EnableCrossPartitionQuery = false
                })
                .Where(e => e.PartitionId == aggregateRootId && e.DocumentType == CommandRegistryWrapper.DocumentTypeName);

            var pagedQuery = Pager.CreatePagedQuery(query);
            while (pagedQuery.HasMoreResults)
            {
                var next = await pagedQuery.ExecuteNextAsync<CommandRegistryWrapper>(cancellationToken);
                foreach (var wrapper in next) list.Add(wrapper.CommandId);
            }

            return list;
        }

        public async Task Save(string aggregateRootId, IEnumerable<Guid> commandIds,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(aggregateRootId))
                throw new ArgumentNullException(nameof(aggregateRootId));

            var requestOptions = new RequestOptions
            {
                PartitionKey = new PartitionKey(aggregateRootId)
            };

            await Task.WhenAll(commandIds.Select(id =>
                Client.UpsertDocumentAsync(Uri, new CommandRegistryWrapper(aggregateRootId, id), requestOptions)));
        }
    }
}