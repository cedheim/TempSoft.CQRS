using System;
using System.Threading;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TempSoft.CQRS.Demo.SF.Configuration;
using TempSoft.CQRS.Projectors;
using TempSoft.CQRS.ServiceFabric.Projectors;

namespace TempSoft.CQRS.Demo.SF.Projectors
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
                var bootstrapper = ServiceFabricStartup.Configure();

                // This line registers an Actor Service to host your actor class with the Service Fabric runtime.
                // The contents of your ServiceManifest.xml and ApplicationManifest.xml files
                // are automatically populated when you build this project.
                // For more information, see https://aka.ms/servicefabricactorsplatform

                ActorRuntime.RegisterActorAsync<ProjectorActor> (
                   (context, actorType) => new ActorService(context, actorType, (service, id) => new ProjectorActor(service, id, bootstrapper.Resolve<IProjectorRepository>(), bootstrapper.Resolve<IActorProxyFactory>(), bootstrapper.Resolve<IServiceProxyFactory>()))).GetAwaiter().GetResult();

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
