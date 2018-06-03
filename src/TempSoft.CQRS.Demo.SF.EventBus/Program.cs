using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Runtime;
using TempSoft.CQRS.Demo.SF.Configuration;
using TempSoft.CQRS.Projectors;
using TempSoft.CQRS.ServiceFabric.Events;
using TempSoft.CQRS.ServiceFabric.Tools;

namespace TempSoft.CQRS.Demo.SF.EventBus
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
                var boostrapper = ServiceFabricStartup.Configure();

                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                ServiceRuntime.RegisterServiceAsync("EventBusServiceType",
                    context => new EventBusService(context, boostrapper.Resolve<IProjectorRegistry>(), boostrapper.Resolve<IUriHelper>(), boostrapper.Resolve<IActorProxyFactory>(), boostrapper.Resolve<IServiceProxyFactory>())).GetAwaiter().GetResult();

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
