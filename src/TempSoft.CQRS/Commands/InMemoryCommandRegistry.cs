﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.Commands
{
    public class InMemoryCommandRegistry : ICommandRegistry
    {
        private readonly ConcurrentDictionary<Guid, ConcurrentBag<Guid>> _db = new ConcurrentDictionary<Guid, ConcurrentBag<Guid>>();

        public Task<IEnumerable<Guid>> Get(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                var bag = _db.GetOrAdd(id, guid => new ConcurrentBag<Guid>());
                return bag.AsEnumerable();
            }, cancellationToken);
        }

        public Task Save(Guid id, IEnumerable<Guid> commandIds, CancellationToken cancellationToken = default(CancellationToken))
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