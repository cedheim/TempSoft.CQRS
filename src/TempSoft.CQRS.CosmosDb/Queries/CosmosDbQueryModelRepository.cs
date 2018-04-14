using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Queries;

namespace TempSoft.CQRS.CosmosDb.Queries
{
    public class CosmosDbQueryModelRepository : RepositoryBase, IQueryModelRepository
    {
        public CosmosDbQueryModelRepository(IDocumentClient client, ICosmosDbQueryPager pager, string databaseId, string collectionId, int initialThroughput = 1000)
            : base(client, pager, databaseId, collectionId, initialThroughput)
        {
        }

        public async Task<T> Get<T>(string id, CancellationToken cancellationToken)
        {
            var response = await Client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
            var wrapper = (QueryModelPayloadWrapper) response.Resource;

            return wrapper.GetPayload<T>();

        }

        public async Task Save<T>(string id, T model, CancellationToken cancellationToken)
        {
            await Client.UpsertDocumentAsync(Uri, new QueryModelPayloadWrapper(id, model));
        }

        public override IEnumerable<string> PartitionKeyPaths => new string[0];
    }
}