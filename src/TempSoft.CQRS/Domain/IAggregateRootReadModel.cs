using System;

namespace TempSoft.CQRS.Domain
{
    public interface IAggregateRootReadModel
    {
        string Id { get; }
        int Version { get; }
    }
}