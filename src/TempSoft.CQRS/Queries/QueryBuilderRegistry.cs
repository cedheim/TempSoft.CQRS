using System;
using System.Collections.Generic;
using System.Linq;

namespace TempSoft.CQRS.Queries
{
    public class QueryBuilderRegistry : IQueryBuilderRegistry
    {
        private readonly Dictionary<Type, IQueryBuilder> _buildersByBuilderType = new Dictionary<Type, IQueryBuilder>();
        private readonly Dictionary<Type, List<IQueryBuilder>> _buildersByEventType = new Dictionary<Type, List<IQueryBuilder>>();
        
        public void Register(IQueryBuilder builder)
        {
            var eventTypes = builder.Events.ToArray();

            foreach (var eventType in eventTypes)
            {
                if (!_buildersByEventType.ContainsKey(eventType))
                {
                    _buildersByEventType.Add(eventType, new List<IQueryBuilder>());
                }

                _buildersByEventType[eventType].Add(builder);
            }

            var builderType = builder.GetType();
            if (!_buildersByBuilderType.ContainsKey(builderType))
            {
                _buildersByBuilderType.Add(builderType, builder);
            }
        }
        
        public IEnumerable<IQueryBuilder> ListQueryBuildersFor(Type eventType)
        {
            return _buildersByEventType.ContainsKey(eventType) 
                ? _buildersByEventType[eventType].AsEnumerable() 
                : Enumerable.Empty<IQueryBuilder>();
        }

        public IQueryBuilder GetQueryBuilderByType(Type builderType)
        {
            return _buildersByBuilderType[builderType];
        }
    }
}