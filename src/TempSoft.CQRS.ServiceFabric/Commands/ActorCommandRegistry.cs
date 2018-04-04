using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using TempSoft.CQRS.Commands;

namespace TempSoft.CQRS.ServiceFabric.Commands
{
    public class ActorCommandRegistry : ICommandRegistry
    {
        private const string CommandLogStateName = "_tempsoft_cqrs_command_log";
        private readonly IActorStateManager _stateManager;

        public ActorCommandRegistry(IActorStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public async Task<IEnumerable<Guid>> Get(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tryGetEventStream = await _stateManager.TryGetStateAsync<Guid[]>(CommandLogStateName, cancellationToken);

            return tryGetEventStream.HasValue
                ? tryGetEventStream.Value
                : Enumerable.Empty<Guid>();
        }

        public async Task Save(Guid id, IEnumerable<Guid> commandIds, CancellationToken cancellationToken = default(CancellationToken))
        {
            var cmds = commandIds.ToArray();

            await _stateManager.AddOrUpdateStateAsync(CommandLogStateName, cmds, (s, guids) => guids.Union(cmds).ToArray(), cancellationToken);
        }
    }
}