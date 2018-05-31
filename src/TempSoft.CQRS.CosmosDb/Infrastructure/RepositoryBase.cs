using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace TempSoft.CQRS.CosmosDb.Infrastructure
{
    public abstract class RepositoryBase
    {
        protected readonly IDocumentClient Client;
        protected readonly string CollectionId;
        protected readonly string DatabaseId;
        protected readonly int InitialThroughput;
        protected readonly ICosmosDbQueryPager Pager;
        protected readonly Uri Uri;

        protected RepositoryBase(IDocumentClient client, ICosmosDbQueryPager pager, string databaseId,
            string collectionId, int initialThroughput = 1000)
        {
            Client = client;
            Pager = pager;
            DatabaseId = databaseId;
            CollectionId = collectionId;
            InitialThroughput = initialThroughput;
            Uri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

            EnsureDatabaseIsInitialized().Wait();
        }

        public abstract IEnumerable<string> PartitionKeyPaths { get; }

        private async Task EnsureDatabaseIsInitialized()
        {
            var database = Client.CreateDatabaseQuery().Where(x => x.Id == DatabaseId).ToArray().FirstOrDefault();

            if (ReferenceEquals(database, default(Database)))
                database = await Client.CreateDatabaseAsync(new Database {Id = DatabaseId});


            var collections = await Client.ReadDocumentCollectionFeedAsync(database.SelfLink);
            var collection = collections.Where(coll => coll.Id == CollectionId).ToArray().FirstOrDefault();

            if (ReferenceEquals(collection, default(DocumentCollection)))
            {
                var collectionSpec = new DocumentCollection
                {
                    Id = CollectionId
                };

                foreach (var path in PartitionKeyPaths) collectionSpec.PartitionKey.Paths.Add(path);

                await Client.CreateDocumentCollectionAsync(database.SelfLink, collectionSpec,
                    new RequestOptions {OfferThroughput = InitialThroughput});
            }
        }
    }
}