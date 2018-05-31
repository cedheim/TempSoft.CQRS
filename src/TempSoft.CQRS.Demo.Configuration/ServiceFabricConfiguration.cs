using System;
using System.Fabric;
using Microsoft.Azure.Documents.Client;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.ServiceFabric.Infrastructure;

namespace TempSoft.CQRS.Demo.Configuration
{
    public static class ServiceFabricStartup
    {
        public static FluentBootstrapper Configure()
        {
            var configuration = new ApplicationSettings(FabricRuntime.GetActivationContext());
            var bootstrapper = new FluentBootstrapper();

            bootstrapper.UseService<IApplicationSettings>(configuration);
            bootstrapper.UseServiceFabric();
            bootstrapper.UseEventBusUri(new Uri("fabric:/TempSoft.CQRS.Demo.Application/EventBusService"));
            bootstrapper.UseProjectorActorrUri(new Uri("fabric:/TempSoft.CQRS.Demo.Application/ProjectorActorService"));
            bootstrapper.UseAggregateRootActorUri(new Uri("fabric:/TempSoft.CQRS.Demo.Application/AggregateRootActorService"));

            bootstrapper.UseCosmosDb(new Uri(configuration["CosmosDb_EndpointUri"]), configuration["CosmosDb_PrimaryKey"]);
            bootstrapper.UseCosmosDbCommandRegistry(configuration["CommandRegistry_DatabaseId"], configuration["CommandRegistry_CollectionId"]);
            bootstrapper.UseCosmosDbEventStore(configuration["EventStore_DatabaseId"], configuration["EventStore_CollectionId"]);
            bootstrapper.UseCosmosDbProjectionModelRepository(configuration["ProjectionModelRepository_DatabaseId"], configuration["ProjectionModelRepository_CollectionId"]);
            
            return bootstrapper;
        }
    }
}