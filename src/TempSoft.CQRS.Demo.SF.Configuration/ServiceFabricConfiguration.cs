using System;
using System.Fabric;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Demo.Infrastructure;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.ServiceFabric.Infrastructure;

namespace TempSoft.CQRS.Demo.SF.Configuration
{
    public static class ServiceFabricStartup
    {
        public static FluentBootstrapper Configure()
        {
            var configuration = new ApplicationSettings(FabricRuntime.GetActivationContext());
            var bootstrapper = BootstrapperGenerator.Generate();

            bootstrapper.UseService<IApplicationSettings>(configuration);
            bootstrapper.UseServiceFabric();
            bootstrapper.UseEventBusUri(new Uri("fabric:/TempSoft.CQRS.Demo.SF.Application/EventBusService"));
            bootstrapper.UseProjectorActorUri(new Uri("fabric:/TempSoft.CQRS.Demo.SF.Application/ProjectorActorService"));
            bootstrapper.UseAggregateRootActorUri(new Uri("fabric:/TempSoft.CQRS.Demo.SF.Application/AggregateRootActorService"));

            bootstrapper.UseCosmosDb(new Uri(configuration["CosmosDb_EndpointUri"]), configuration["CosmosDb_PrimaryKey"]);
            bootstrapper.UseCosmosDbCommandRegistry(configuration["CommandRegistry_DatabaseId"], configuration["CommandRegistry_CollectionId"]);
            bootstrapper.UseCosmosDbEventStore(configuration["EventStore_DatabaseId"], configuration["EventStore_CollectionId"]);
            bootstrapper.UseCosmosDbProjectionModelRepository(configuration["ProjectionModelRepository_DatabaseId"], configuration["ProjectionModelRepository_CollectionId"]);

            bootstrapper.Validate();

            return bootstrapper;
        }
    }
}