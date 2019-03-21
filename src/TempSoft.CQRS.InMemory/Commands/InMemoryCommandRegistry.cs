using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Commands;

namespace TempSoft.CQRS.InMemory.Commands
{
    public class InMemoryCommandRegistry : ICommandRegistry
    {
        private readonly ConcurrentDictionary<string, ConcurrentBag<Guid>> _db = new ConcurrentDictionary<string, ConcurrentBag<Guid>>();

        public Task<IEnumerable<Guid>> Get(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                var bag = _db.GetOrAdd(id, guid => new ConcurrentBag<Guid>());
                return bag.AsEnumerable();
            }, cancellationToken);
        }

        public Task Save(string id, IEnumerable<Guid> commandIds, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                var bag = _db.GetOrAdd(id, guid => new ConcurrentBag<Guid>());
                foreach(var commandId in commandIds)
                    bag.Add(commandId);
            }, cancellationToken);
        }
    }
}