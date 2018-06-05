using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Projectors
{
    public interface IProjectorActor : IActor
    {
        Task Project(ProjectorMessage message, CancellationToken cancellationToken);

        Task<QueryResultMessage> Query(QueryMessage message, CancellationToken cancellationToken);
    }
}