using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Exceptions;
using TempSoft.CQRS.Extensions;

namespace TempSoft.CQRS.Projectors
{
    public abstract class ProjectorBase<T> : IProjector where T : ProjectorBase<T>
    {
        private static readonly Dictionary<Type, Func<T, IEvent, CancellationToken, Task>> Projectors = new Dictionary<Type, Func<T, IEvent, CancellationToken, Task>>();
        private static readonly Dictionary<Type, Func<T, IQuery, CancellationToken, Task<IQueryResult>>> Queries = new Dictionary<Type, Func<T, IQuery, CancellationToken, Task<IQueryResult>>>();

        static ProjectorBase()
        {
            InitializeProjectors();
            InitializeQueries();
        }

        public string ProjectorId { get; set; }

        public async Task Project(IEvent @event, CancellationToken cancellationToken)
        {
            var type = @event.GetType();
            if (Projectors.TryGetValue(type, out var caller))
            {
                await caller((T)this, @event, cancellationToken);
            }
            else
            {
                throw new MissingProjectorException($"No projector found on {typeof(T).Name} for event {type.Name}");
            }
        }

        public async Task<IQueryResult> Query(IQuery query, CancellationToken cancellationToken)
        {
            var type = query.GetType();
            if (Queries.TryGetValue(type, out var caller))
            {
                return await caller((T)this, query, cancellationToken);
            }
            else
            {
                throw new MissingProjectorException($"No projector found on {typeof(T).Name} for event {type.Name}");
            }
        }


        private static void InitializeProjectors()
        {
            foreach (var projector in typeof(T).GetMethodsForAttribute<ProjectorAttribute>())
            foreach (var projectorAttribute in projector.GetCustomAttributes(typeof(ProjectorAttribute), true).Cast<ProjectorAttribute>())
            {
                var caller = projector.GenerateAsyncHandler<T, IEvent>(projectorAttribute.For);
                Projectors.Add(projectorAttribute.For, caller);
            }
        }

        private static void InitializeQueries()
        {
            foreach (var query in typeof(T).GetMethodsForAttribute<QueryAttribute>())
            foreach (var queryAttribute in query.GetCustomAttributes(typeof(QueryAttribute), true).Cast<QueryAttribute>())
            {
                var caller = query.GenerateAsyncHandlerWithReturnType<T, IQuery, IQueryResult>(queryAttribute.For);
                Queries.Add(queryAttribute.For, caller);
            }
        }
    }
}