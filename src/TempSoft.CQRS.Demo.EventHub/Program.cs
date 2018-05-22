﻿using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Runtime;
using TempSoft.CQRS.Demo.Common.Configuration;
using Microsoft.Azure.Documents.Client;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.ServiceFabric.Events;
using TempSoft.CQRS.ServiceFabric.Tools;

namespace TempSoft.CQRS.Demo.EventHub
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                var actorProxyFactory = new ActorProxyFactory();
                var serviceProxyFactory = new ServiceProxyFactory();
                var settings = new ApplicationSettings(FabricRuntime.GetActivationContext());
                var uriBuilder = new ApplicationUriBuilder(FabricRuntime.GetActivationContext());

                var eventStoreClient = new DocumentClient(new Uri(settings["EventStore_EndpointUri"]), settings["EventStore_PrimaryKey"]);
                var eventStreamStateClient = new DocumentClient(new Uri(settings["EventStreamState_EndpointUri"]), settings["EventStreamState_PrimaryKey"]);

                var eventStreamRegistry = new EventStreamRegistry(new EventStreamDefinition[]
                {
                    new EventStreamDefinition("TestStream", new EventFilter()), 
                });


                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                ServiceRuntime.RegisterServiceAsync("EventHubServiceType",
                    context => new EventBusService(context, eventStreamRegistry, uriBuilder, serviceProxyFactory, actorProxyFactory)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(EventBusService).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
