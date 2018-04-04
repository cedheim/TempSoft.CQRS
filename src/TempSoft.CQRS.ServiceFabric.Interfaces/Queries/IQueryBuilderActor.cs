using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Queries
{
    public interface IQueryBuilderActor : IActor
    {
        Task Apply(EventMessage message, CancellationToken cancellationToken);
    }
}