using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TempSoft.CQRS.Projectors;
using TempSoft.CQRS.ServiceFabric.Exceptions;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.ServiceFabric.Interfaces.Projectors;

namespace TempSoft.CQRS.ServiceFabric.Projectors
{
    [StatePersistence(StatePersistence.None)]
    public class ProjectorActor : Actor, IProjectorActor
    {
        private readonly IProjectorRepository _repository;
        private readonly IActorProxyFactory _actorProxyFactory;
        private readonly IServiceProxyFactory _serviceProxyFactory;
        private readonly string _projectorId;

        private IProjector _projector;

        public ProjectorActor(ActorService actorService, ActorId actorId, IProjectorRepository repository, IActorProxyFactory actorProxyFactory, IServiceProxyFactory serviceProxyFactory) : base(actorService, actorId)
        {
            _projector = default(IProjector);
            _projectorId = actorId.GetStringId();
            _repository = repository;
            _actorProxyFactory = actorProxyFactory;
            _serviceProxyFactory = serviceProxyFactory;
        }

        public async Task Project(ProjectorMessage message, CancellationToken cancellationToken)
        {
            if (object.ReferenceEquals(_projector, default(IProjector)))
            {
                _projector = await _repository.Get(message.ProjectorType, _projectorId, cancellationToken);
            }
            else if (_projector.GetType() != message.ProjectorType)
            {
                throw new ProjectorTypeMissmatchException($"Projector actor {_projectorId} message was for type {message.ProjectorType} but local projector is of type {_projector.GetType()}.");
            }

            await _projector.Project(message.Body, cancellationToken);
        }

        public async Task<QueryResultMessage> Query(QueryMessage message, CancellationToken cancellationToken)
        {
            if (object.ReferenceEquals(_projector, default(IProjector)))
            {
                _projector = await _repository.Get(message.ProjectorType, _projectorId, cancellationToken);
            }
            else if (_projector.GetType() != message.ProjectorType)
            {
                throw new ProjectorTypeMissmatchException($"Projector actor {_projectorId} message was for type {message.ProjectorType} but local projector is of type {_projector.GetType()}.");
            }

            var query = message.Body;
            var result = await _projector.Query(query, cancellationToken);

            return new QueryResultMessage(result);
        }
    }
}