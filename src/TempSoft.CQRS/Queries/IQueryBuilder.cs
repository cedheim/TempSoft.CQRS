using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Queries
{
    public interface IQueryBuilder
    {
        IEnumerable<Type> Events { get; }

        Task Apply(IEvent @event, CancellationToken cancellationToken);
    }
}