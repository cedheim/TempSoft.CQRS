﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace NCG.NGS.CQRS.Domain
{
    public interface IRepository
    {
        Task<IAggregateRoot> Get(Type type, Guid id, CancellationToken cancellationToken = default(CancellationToken));

        Task<TAggregate> Get<TAggregate>(Guid id, CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot;

        Task Save(IAggregateRoot root, CancellationToken cancellationToken = default(CancellationToken));
    }
}