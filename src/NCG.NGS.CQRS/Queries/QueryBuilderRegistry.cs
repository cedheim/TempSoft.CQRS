using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCG.NGS.CQRS.Events;

namespace NCG.NGS.CQRS.Queries
{
    public class QueryBuilderRegistry : IQueryBuilderRegistry
    {
        private readonly Dictionary<Type, List<IQueryBuilder>> _builders = new Dictionary<Type, List<IQueryBuilder>>();
        
        public void Register(IQueryBuilder builder)
        {
            var eventTypes = builder.Events.ToArray();

            foreach (var eventType in eventTypes)
            {
                if (!_builders.ContainsKey(eventType))
                {
                    _builders.Add(eventType, new List<IQueryBuilder>());
                }

                _builders[eventType].Add(builder);
            }

        }
        
        public IEnumerable<IQueryBuilder> ListQueryBuildersFor(Type eventType)
        {
            return _builders.ContainsKey(eventType) 
                ? _builders[eventType].AsEnumerable() 
                : Enumerable.Empty<IQueryBuilder>();
        }
    }
}