using System;

namespace NCG.NGS.CQRS.Domain
{
    public interface IAggregateRootReadModel
    {
        Guid Id { get; }
        int Version { get; }
    }
}