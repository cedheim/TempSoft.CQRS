using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Exceptions;
using TempSoft.CQRS.Extensions;

namespace TempSoft.CQRS.Queries
{
    public abstract class QueryBuilderBase<T> : IQueryBuilder where T : QueryBuilderBase<T>
    {
        private static readonly Dictionary<Type, Func<T, IEvent, CancellationToken, Task>> QueryBuilders = new Dictionary<Type, Func<T, IEvent, CancellationToken, Task>>();

        static QueryBuilderBase()
        {
            InitializeQueryBuilders();
        }

        protected QueryBuilderBase(IQueryModelRepository repository)
        {
            Repository = repository;
        }

        public IQueryModelRepository Repository { get; }

        public abstract IEnumerable<Type> Events { get; }

        public async Task Apply(IEvent @event, CancellationToken cancellationToken)
        {
            if (!QueryBuilders.TryGetValue(@event.GetType(), out var builder))
            {
                throw new MissingQueryBuilderHandlerException($"Missing query builder handler for event type {@event.GetType().Name} in {typeof(T).Name}");
            }

            await builder((T) this, @event, cancellationToken);
        }

        private static void InitializeQueryBuilders()
        {
            foreach (var queryBuilder in typeof(T).GetMethodsForAttribute<QueryBuilderHandlerAttribute>())
            {
                foreach (var queryBuilderAttribute in queryBuilder.GetCustomAttributes(typeof(QueryBuilderHandlerAttribute), true).Cast<QueryBuilderHandlerAttribute>())
                {
                    var caller = queryBuilder.GenerateAsyncHandler<T, IEvent>(queryBuilderAttribute.For);
                    QueryBuilders.Add(queryBuilderAttribute.For, caller);
                }
            }
        }
    }
}