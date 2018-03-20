using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;

namespace NCG.NGS.CQRS.ServiceFabric.Interfaces.Queries
{
    public interface IQueryBuilderActor : IActor
    {
        Task Apply(EventMessage message, CancellationToken cancellationToken);
    }
}