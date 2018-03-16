using System;
using System.Collections;
using System.Collections.Generic;
using NCG.NGS.CQRS.Commands;
using NCG.NGS.CQRS.Events;

namespace NCG.NGS.CQRS.Domain
{
    public interface IAggregateRoot
    {
        Guid Id { get; }

        int Version { get; }
        
        void Initialize(Guid id);

        void Handle(ICommand command);

        void LoadFrom(IEnumerable<IEvent> events);

        IEnumerable<IEvent> Commit();



        void ApplyChange(IEvent @event);
    }
}