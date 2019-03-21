using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.Commands
{
    public interface ICommandRegistry
    {
        Task<IEnumerable<Guid>> Get(string id, CancellationToken cancellationToken = default(CancellationToken));

        Task Save(string id, IEnumerable<Guid> commandIds,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}