using System;
using System.Collections.Generic;

namespace NCG.NGS.CQRS.Queries
{
    public interface IQueryBuilder
    {
        IEnumerable<Type> Events { get; }
    }
}