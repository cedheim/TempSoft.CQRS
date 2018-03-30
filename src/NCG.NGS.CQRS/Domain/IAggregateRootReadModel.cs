using System;

namespace TempSoft.CQRS.Domain
{
    public interface IAggregateRootReadModel
    {
        Guid Id { get; }
        int Version { get; }
    }
}