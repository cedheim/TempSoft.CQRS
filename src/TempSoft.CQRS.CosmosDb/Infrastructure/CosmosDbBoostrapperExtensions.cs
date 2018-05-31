using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.CosmosDb.Commands;
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Projectors;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.CosmosDb.Infrastructure
{
    public static class CosmosDbBoostrapperExtensions
    {
        public static FluentBootstrapper UseCosmosDb(this FluentBootstrapper bootstrapper, Uri endpointUri, string primaryKey, ConnectionPolicy connectionPolicy = default(ConnectionPolicy), ConsistencyLevel? consistencyLevel = default(ConsistencyLevel?))
        {
            bootstrapper.UseService<IDocumentClient>(() => new DocumentClient(endpointUri, primaryKey, connectionPolicy, consistencyLevel));
            bootstrapper.UseService<ICosmosDbQueryPager, CosmosDbQueryPager>(true);
            return bootstrapper;
        }

        public static FluentBootstrapper UseCosmosDbCommandRegistry(this FluentBootstrapper bootstrapper, string databaseId, string collectionId, int initialThroughput = 1000)
        {
            var pager = bootstrapper.Resolve<ICosmosDbQueryPager>();
            var client = bootstrapper.Resolve<IDocumentClient>();
            var registry  = new CosmosDbCommandRegistry(client, pager, databaseId, collectionId, initialThroughput);

            bootstrapper.UseService<ICommandRegistry>(registry);

            return bootstrapper;
        }

        public static FluentBootstrapper UseCosmosDbEventStore(this FluentBootstrapper bootstrapper, string databaseId, string collectionId, int initialThroughput = 1000)
        {
            var pager = bootstrapper.Resolve<ICosmosDbQueryPager>();
            var client = bootstrapper.Resolve<IDocumentClient>();
            var store = new CosmosDbEventStore(client, pager, databaseId, collectionId, initialThroughput);

            bootstrapper.UseService<IEventStore>(store);

            return bootstrapper;
        }
        
        public static FluentBootstrapper UseCosmosDbProjectionModelRepository(this FluentBootstrapper bootstrapper, string databaseId, string collectionId, int initialThroughput = 1000)
        {
            var pager = bootstrapper.Resolve<ICosmosDbQueryPager>();
            var client = bootstrapper.Resolve<IDocumentClient>();
            var repository = new CosmosDbProjectionModelRepository(client, pager, databaseId, collectionId, initialThroughput);

            bootstrapper.UseService<IProjectionModelRepository>(repository);

            return bootstrapper;
        }
    }
}