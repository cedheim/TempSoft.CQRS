using System;
using System.Collections.Generic;

namespace TempSoft.CQRS.Queries
{
    public interface IQueryBuilderRegistry
    {
        void Register(IQueryBuilder builder);

        IEnumerable<IQueryBuilder> ListQueryBuildersFor(Type eventType);

        IQueryBuilder GetQueryBuilderByType(Type queryBuilder);
    }
}