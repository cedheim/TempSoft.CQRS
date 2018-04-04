using System;
using System.Collections.Generic;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Domain
{
    public interface IAggregateRoot
    {
        Guid Id { get; }

        int Version { get; }
        
        void Initialize(Guid id);
        
        void Handle(ICommand command);

        void LoadFrom(IEnumerable<IEvent> events, IEnumerable<Guid> commandIds);

        Commit Commit();
        
        void ApplyChange(IEvent @event);
    }
}