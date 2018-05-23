using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using Microsoft.Azure.Documents.Client;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TempSoft.CQRS.Common.Uri;
using TempSoft.CQRS.CosmosDb.Commands;
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.ServiceFabric.Domain;
using TempSoft.CQRS.ServiceFabric.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;

namespace TempSoft.CQRS.Demo.AggregateRoots
{
    internal static class Program
    {
        /// <summary>
        ///     This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // This line registers an Actor Service to host your actor class with the Service Fabric runtime.
                // The contents of your ServiceManifest.xml and ApplicationManifest.xml files
                // are automatically populated when you build this project.
                // For more information, see https://aka.ms/servicefabricactorsplatform
                var actorProxyFactory = new ActorProxyFactory();
                var serviceProxyFactory = new ServiceProxyFactory();
                var uriHelper = new UriHelper();
                var eventBus = new ServiceFabricEventBus(serviceProxyFactory, uriHelper);
                var configuration = GenerateConfiguration(FabricRuntime.GetActivationContext());
                var eventStoreClient = new DocumentClient(new Uri(configuration["EventStore_EndpointUri"]),
                    configuration["EventStore_PrimaryKey"]);
                var commandRegistryClient = new DocumentClient(new Uri(configuration["CommandRegistry_EndpointUri"]),
                    configuration["CommandRegistry_PrimaryKey"]);

                commandRegistryClient.OpenAsync().Wait();
                eventStoreClient.OpenAsync().Wait();

                uriHelper.RegisterUri<IEventBusService>(
                    new Uri("fabric:/TempSoft.CQRS.Demo.Application/EventBusService"));
                uriHelper.RegisterUri<IEventStreamService>(
                    new Uri("fabric:/TempSoft.CQRS.Demo.Application/EventStreamService"));

                var eventStore = new CosmosDbEventStore(eventStoreClient, new CosmosDbQueryPager(),
                    configuration["EventStore_DatabaseId"], configuration["EventStore_CollectionId"]);
                var commandRegistry = new CosmosDbCommandRegistry(commandRegistryClient, new CosmosDbQueryPager(),
                    configuration["CommandRegistry_DatabaseId"], configuration["CommandRegistry_CollectionId"]);


                ActorRuntime.RegisterActorAsync<AggregateRootActor>(
                    (context, actorType) => new ActorService(
                        context,
                        actorType,
                        (service, id) => new AggregateRootActor(service, id,
                            actor => new AggregateRootRepository(eventStore, eventBus, commandRegistry),
                            actorProxyFactory, serviceProxyFactory))
                ).GetAwaiter().GetResult();

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static IDictionary<string, string> GenerateConfiguration(ICodePackageActivationContext context)
        {
            var configuration = new Dictionary<string, string>();

            var sections = context.GetConfigurationPackageObject("Config")?.Settings?.Sections;
            if (sections == null)
                return configuration;

            foreach (var section in sections)
            {
                var parameters = section?.Parameters;
                if (parameters == null)
                    continue;

                foreach (var parameter in parameters)
                {
                    if (configuration.ContainsKey(parameter.Name))
                        continue;

                    configuration.Add(parameter.Name, parameter.Value);
                }
            }

            return configuration;
        }
    }
}