using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Domain
{
    public interface IAggregateRoot
    {
        Guid Id { get; set; }

        int Version { get; }

        Task Handle(ICommand command, CancellationToken cancellationToken);

        void LoadFrom(IEnumerable<IEvent> events, IEnumerable<Guid> commandIds);

        Commit Commit();

        void ApplyChange(IEvent @event);
    }
}