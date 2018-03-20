using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NCG.NGS.CQRS.Events;

namespace NCG.NGS.CQRS.Queries
{
    public interface IQueryBuilderRegistry
    {
        void Register(IQueryBuilder builder);

        IEnumerable<IQueryBuilder> ListQueryBuildersFor(Type eventType);
    }
}