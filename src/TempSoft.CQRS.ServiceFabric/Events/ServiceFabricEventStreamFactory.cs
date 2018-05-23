using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TempSoft.CQRS.Common.Uri;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.ServiceFabric.Exceptions;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;

namespace TempSoft.CQRS.ServiceFabric.Events
{
    public class ServiceFabricEventStreamFactory : IEventStreamFactory
    {
        private readonly IServiceProxyFactory _serviceProxyFactory;
        private readonly IEventStreamRegistry _streamRegistry;
        private readonly IUriHelper _uriHelper;

        public ServiceFabricEventStreamFactory(IUriHelper uriHelper, IEventStreamRegistry streamRegistry,
            IServiceProxyFactory serviceProxyFactory)
        {
            _uriHelper = uriHelper;
            _streamRegistry = streamRegistry;
            _serviceProxyFactory = serviceProxyFactory;
        }

        public async Task<IEventStream> Open(string name)
        {
            var definition = _streamRegistry.GetEventStreamByName(name);
            if (ReferenceEquals(definition, default(EventStreamDefinition)))
                throw new EventStreamNotFoundException($"Could not find event stream with name {name}");

            var proxy = _serviceProxyFactory.CreateServiceProxy<IEventStreamService>(
                _uriHelper.GetUriForSerivce<IEventStreamService>(), new ServicePartitionKey(definition.Name));
            return new ServiceFabricEventStream(definition, proxy);
        }
    }
}