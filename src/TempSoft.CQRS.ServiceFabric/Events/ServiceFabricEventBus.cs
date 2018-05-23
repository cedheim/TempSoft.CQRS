using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TempSoft.CQRS.Common.Extensions;
using TempSoft.CQRS.Common.Uri;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Events
{
    public class ServiceFabricEventBus : IEventBus
    {
        private readonly IServiceProxyFactory _proxyFactory;
        private readonly IUriHelper _uriHelper;

        public ServiceFabricEventBus(IServiceProxyFactory proxyFactory, IUriHelper uriHelper)
        {
            _proxyFactory = proxyFactory;
            _uriHelper = uriHelper;
        }

        public async Task Publish(IEnumerable<IEvent> events, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

            //var tasks =
            //    from eventGroup in events.GroupBy(e => e.AggregateRootId)
            //    let hash = eventGroup.Key.GetHashCode64()
            //    let messages = eventGroup.Select(e => new EventMessage(e)).OrderBy(e => e.Body.Version).ToArray()
            //    let spk = new ServicePartitionKey(hash)
            //    let proxy = _proxyFactory.CreateServiceProxy<IEventBusService>(_uri, spk)
            //    select proxy.Publish(messages, cancellationToken);

            //await Task.WhenAll(tasks);
        }
    }
}