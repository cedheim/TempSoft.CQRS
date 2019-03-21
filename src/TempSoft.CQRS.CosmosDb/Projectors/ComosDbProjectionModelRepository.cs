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

        public override IEnumerable<string> PartitionKeyPaths => new[] { "/PartitionId" };

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
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrEmpty(projectorId))
                throw new ArgumentNullException(nameof(projectorId));

            try
            {
                var documentId = ProjectionPayloadWrapper.CreateIdentifier(id);
                var documentUri = UriFactory.CreateDocumentUri(DatabaseId, CollectionId, documentId);
                var requestOptions = new RequestOptions()
                {
                    PartitionKey = new PartitionKey(projectorId)
                };

                var document = (await Client.ReadDocumentAsync(documentUri, requestOptions)).Resource;
                var wrapper = (ProjectionPayloadWrapper)document;
                var projection = wrapper.GetProjection();

                return (TProjectionModel)projection;
            }
            catch (DocumentClientException)
            {
                return default(TProjectionModel);
            }
        }

        public async Task List(string projectorId, Func<IProjection, CancellationToken, Task> callback, int skip, int take, CancellationToken cancellationToken)
        {
            var feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(projectorId),
                MaxDegreeOfParallelism = -1
            };

            var collectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
            IQueryable<ProjectionPayloadWrapper> query = Client.CreateDocumentQuery<ProjectionPayloadWrapper>(collectionUri, feedOptions)
                .Where(w => w.PartitionId == projectorId && w.DocumentType == ProjectionPayloadWrapper.DocumentTypeName)
                .OrderBy(w => w.Epoch);

            if (skip < 0)
            {
                skip = 0;
            }

            var count = 0;
            using (var pagedQuery = Pager.CreatePagedQuery(query))
            {

                while (pagedQuery.HasMoreResults && !cancellationToken.IsCancellationRequested)
                {
                    var next = (await pagedQuery.ExecuteNextAsync<ProjectionPayloadWrapper>(cancellationToken));

                    foreach (var wrapper in next)
                    {
                        if (count >= skip)
                        {
                            if (take < 0)
                            {
                               await callback(wrapper.GetProjection(), cancellationToken);
                            }
                            else if (count < (skip + take)) 
                            {
                                await callback(wrapper.GetProjection(), cancellationToken);
                            }
                            else
                            {
                                return;
                            }


                        }



                        ++count;
                    }
                }
            }


        }

        public Task List(string projectorId, Func<IProjection, CancellationToken, Task> callback, CancellationToken cancellationToken)
        {
            return List(projectorId, callback, -1, -1, cancellationToken);
        }
    }
}