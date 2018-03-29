using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;

namespace NCG.NGS.CQRS.ServiceFabric.Interfaces.Domain
{
    public interface IAggregateRootActor : IActor
    {
        Task Initialize(InitializeMessage message, CancellationToken cancellationToken);

        Task Handle(CommandMessage message, CancellationToken cancellationToken);

        Task<ReadModelMessage> GetReadModel();
    }
}