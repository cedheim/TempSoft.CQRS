using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.CosmosDb.Projectors
{
    public class CosmosDbProjectionModelRepository : RepositoryBase, IProjectionModelRepository
    {
        public CosmosDbProjectionModelRepository(IDocumentClient client, ICosmosDbQueryPager pager, string databaseId, string collectionId, int initialThroughput = 1000) 
            : base(client, pager, databaseId, collectionId, initialThroughput)
        {
        }

        public override IEnumerable<string> PartitionKeyPaths => new[]{ "/ProjectorId" };

        public async Task Save(IProjection model, CancellationToken cancellationToken)
        {
            var collectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
            var requestOptions = new RequestOptions()
            {
                PartitionKey = new PartitionKey(model.ProjectorId)
            };

            var wrapper = new ProjectionPayloadWrapper(model);
            await Client.UpsertDocumentAsync(collectionUri, wrapper, requestOptions);
        }

        public async Task<TProjectionModel> Get<TProjectionModel>(string id, string projectorId, CancellationToken cancellationToken) where TProjectionModel : IProjection
        {
            try
            {
                var documentUri = UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id);
                var requestOptions = new RequestOptions()
                {
                    PartitionKey = new PartitionKey(projectorId)
                };

                var document = (await Client.ReadDocumentAsync(documentUri, requestOptions)).Resource;
                var wrapper = (ProjectionPayloadWrapper) document;

                var projection = wrapper.GetProjection();
                return (TProjectionModel) projection;
            }
            catch (DocumentClientException)
            {
                return default(TProjectionModel);
            }
        }

        public async Task List(string projectorId, Func<IProjection, CancellationToken, Task> callback, CancellationToken cancellationToken)
        {
            var feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(projectorId),
                MaxDegreeOfParallelism = -1
            };

            var collectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
            var query = Client.CreateDocumentQuery<ProjectionPayloadWrapper>(collectionUri, feedOptions)
                .Where(w => w.ProjectorId == projectorId);

            var pagedQuery = Pager.CreatePagedQuery(query);
            
            while (pagedQuery.HasMoreResults && !cancellationToken.IsCancellationRequested)
            {
                var next = (await pagedQuery.ExecuteNextAsync<ProjectionPayloadWrapper>(cancellationToken));

                foreach (var wrapper in next)
                {
                    await callback(wrapper.GetProjection(), cancellationToken);
                }
            }

        }
    }
}