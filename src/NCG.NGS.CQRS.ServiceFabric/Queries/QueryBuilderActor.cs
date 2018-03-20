using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Queries;

namespace NCG.NGS.CQRS.ServiceFabric.Queries
{
    public class QueryBuilderActor : Actor, IQueryBuilderActor
    {
        public QueryBuilderActor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        public Task Apply(EventMessage message, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}