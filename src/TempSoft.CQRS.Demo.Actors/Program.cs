using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TempSoft.CQRS.Common.Uri;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.ServiceFabric.Commands;
using TempSoft.CQRS.ServiceFabric.Domain;
using TempSoft.CQRS.ServiceFabric.Events;

namespace TempSoft.CQRS.Demo.Actors
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
                // This line registers an Actor Service to host your actor class with the Service Fabric runtime.
                // The contents of your ServiceManifest.xml and ApplicationManifest.xml files
                // are automatically populated when you build this project.
                // For more information, see https://aka.ms/servicefabricactorsplatform
                var actorProxyFactory = new ActorProxyFactory();
                var serviceProxyFactory = new ServiceProxyFactory();
                var uriHelper = new UriHelper();
                var eventBus = new ServiceFabricEventBus(serviceProxyFactory, uriHelper);

                ActorRuntime.RegisterActorAsync<AggregateRootActor> (
                    (context, actorType) => new ActorService(
                        context, 
                        actorType, 
                        (service, id) => new AggregateRootActor(service, id, actor => new AggregateRootRepository(new ActorEventStore(actor.StateManager), eventBus, new ActorCommandRegistry(actor.StateManager)), actorProxyFactory, serviceProxyFactory))
                    ).GetAwaiter().GetResult();

                Thread.Sleep(Timeout.Infinite);
            }
            catch (System.Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
