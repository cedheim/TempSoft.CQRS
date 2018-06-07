using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using TempSoft.CQRS.Projectors;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.ServiceFabric.Interfaces.Projectors;
using TempSoft.CQRS.ServiceFabric.Tools;

namespace TempSoft.CQRS.ServiceFabric.Projectors
{
    public class ServiceFabricProjectionQueryRouter : IProjectionQueryRouter
    {
        private readonly IActorProxyFactory _actorProxyFactory;
        private readonly IUriHelper _uriHelper;

        public ServiceFabricProjectionQueryRouter(IActorProxyFactory actorProxyFactory, IUriHelper uriHelper)
        {
            _actorProxyFactory = actorProxyFactory;
            _uriHelper = uriHelper;
        }

        public async Task<TQueryResult> SendQuery<TProjector, TQueryResult>(IQuery query, string projectorId, CancellationToken cancellationToken) where TProjector : IProjector where TQueryResult : IQueryResult
        {
            var uri = _uriHelper.GetUriFor<IProjectorActor>();
            var proxy = _actorProxyFactory.CreateActorProxy<IProjectorActor>(uri, new ActorId(projectorId));
            var message = new QueryMessage(query, typeof(TProjector));

            var result = await proxy.Query(message, cancellationToken);
            if (object.ReferenceEquals(result, default(QueryResultMessage)))
            {
                return default(TQueryResult);
            }

            return result.GetQueryResult<TQueryResult>();
        }
    }
}