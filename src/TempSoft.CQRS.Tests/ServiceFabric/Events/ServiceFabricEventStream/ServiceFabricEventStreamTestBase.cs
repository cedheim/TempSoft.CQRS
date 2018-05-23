using System;
using FakeItEasy;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TempSoft.CQRS.Common.Uri;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;

namespace TempSoft.CQRS.Tests.ServiceFabric.Events.ServiceFabricEventStream
{
    public abstract class ServiceFabricEventStreamTestBase
    {
        protected readonly IUriHelper UriHelper = A.Fake<IUriHelper>();
        protected readonly IEventStreamRegistry StreamRegistry = A.Fake<IEventStreamRegistry>();
        protected readonly IServiceProxyFactory ServiceProxyFactory = A.Fake<IServiceProxyFactory>();
        protected readonly IEventStreamService StreamService = A.Fake<IEventStreamService>();

        protected readonly Uri StreamServiceUri = new Uri("fabric:/Application/EventStreamService");
        
        protected ServiceFabricEventStreamTestBase()
        {
            A.CallTo(() => ServiceProxyFactory.CreateServiceProxy<IEventStreamService>(A<Uri>.Ignored, A<ServicePartitionKey>.Ignored, A<TargetReplicaSelector>.Ignored, A<string>.Ignored))
                .Returns(StreamService);
            A.CallTo(() => UriHelper.GetUriForSerivce<IEventStreamService>())
                .Returns(StreamServiceUri);
        }


    }
}