using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TempSoft.CQRS.Queries;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.ServiceFabric.Interfaces.Queries;

namespace TempSoft.CQRS.ServiceFabric.Queries
{
    public class QueryBuilderActor : Actor, IQueryBuilderActor
    {
        private readonly IQueryBuilderRegistry _registry;
        private IQueryBuilder _builder;

        public QueryBuilderActor(ActorService actorService, ActorId actorId, IQueryBuilderRegistry registry,
            IActorProxyFactory actorProxyFactory, IServiceProxyFactory serviceProxyFactory) : base(actorService,
            actorId)
        {
            ActorProxyFactory = actorProxyFactory;
            ServiceProxyFactory = serviceProxyFactory;

            _registry = registry;
        }

        public IActorProxyFactory ActorProxyFactory { get; }
        public IServiceProxyFactory ServiceProxyFactory { get; }

        public async Task Apply(EventMessage message, CancellationToken cancellationToken)
        {
            var builder = GetQueryBuilder();

            await builder.Apply(message.Body, cancellationToken);
        }

        private IQueryBuilder GetQueryBuilder()
        {
            if (_builder == null)
            {
                var builderType = Type.GetType(this.GetActorId().GetStringId());
                _builder = _registry.GetQueryBuilderByType(builderType);
            }

            return _builder;
        }
    }
}