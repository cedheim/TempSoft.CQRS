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
        public CosmosDbCommandRegistry(IDocumentClient client, ICosmosDbQueryPager pager, string databaseId, string collectionId, int initialThroughput = 1000)
            : base(client, pager, databaseId, collectionId, initialThroughput)
        {
        }

        public override IEnumerable<string> PartitionKeyPaths => new string[] {"/AggregateRootId"};

        public async Task<IEnumerable<Guid>> Get(Guid aggregateRootId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var list = new List<Guid>();
            var query = Client.CreateDocumentQuery<CommandRegistryWrapper>(Uri, new FeedOptions(){ PartitionKey = new PartitionKey(aggregateRootId.ToString()), MaxDegreeOfParallelism = -1, EnableCrossPartitionQuery = false })
                .Where(cmd => cmd.AggregateRootId == aggregateRootId);

            var pagedQuery = Pager.CreatePagedQuery(query);
            while (pagedQuery.HasMoreResults)
            {
                var next = await pagedQuery.ExecuteNextAsync<CommandRegistryWrapper>(cancellationToken);
                foreach (var wrapper in next)
                {
                    list.Add(wrapper.Id);
                }

            }

            return list;
        }

        public async Task Save(Guid aggregateRootId, IEnumerable<Guid> commandIds, CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestOptions = new RequestOptions()
            {
                PartitionKey = new PartitionKey(aggregateRootId.ToString())
            };

            await Task.WhenAll(commandIds.Select(id => Client.UpsertDocumentAsync(Uri, new CommandRegistryWrapper(aggregateRootId, id), requestOptions)));
        }
    }
}