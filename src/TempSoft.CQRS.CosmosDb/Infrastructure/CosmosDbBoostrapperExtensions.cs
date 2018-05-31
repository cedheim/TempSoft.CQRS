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
            bootstrapper.UseService<ICosmosDbQueryPager, CosmosDbQueryPager>(true);
            bootstrapper.UseService<IDocumentClient>(() => new DocumentClient(endpointUri, primaryKey, connectionPolicy, consistencyLevel), true);
            return bootstrapper;
        }

        public static FluentBootstrapper UseCosmosDbCommandRegistry(this FluentBootstrapper bootstrapper, string databaseId, string collectionId, int initialThroughput = 1000)
        {
            bootstrapper.UseService<ICommandRegistry>(() =>
            {
                var pager = bootstrapper.Resolve<ICosmosDbQueryPager>();
                var client = bootstrapper.Resolve<IDocumentClient>();
                return new CosmosDbCommandRegistry(client, pager, databaseId, collectionId, initialThroughput);
            }, true);

            return bootstrapper;
        }

        public static FluentBootstrapper UseCosmosDbEventStore(this FluentBootstrapper bootstrapper, string databaseId, string collectionId, int initialThroughput = 1000)
        {
            bootstrapper.UseService<IEventStore>(() =>
            {
                var pager = bootstrapper.Resolve<ICosmosDbQueryPager>();
                var client = bootstrapper.Resolve<IDocumentClient>();
                return new CosmosDbEventStore(client, pager, databaseId, collectionId, initialThroughput);
            }, true);

            return bootstrapper;
        }
        
        public static FluentBootstrapper UseCosmosDbProjectionModelRepository(this FluentBootstrapper bootstrapper, string databaseId, string collectionId, int initialThroughput = 1000)
        {
            bootstrapper.UseService<IProjectionModelRepository>(() =>
            {
                var pager = bootstrapper.Resolve<ICosmosDbQueryPager>();
                var client = bootstrapper.Resolve<IDocumentClient>();
                return new CosmosDbProjectionModelRepository(client, pager, databaseId, collectionId, initialThroughput);
            }, true);

            return bootstrapper;
        }
    }
}