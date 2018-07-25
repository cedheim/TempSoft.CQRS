using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.InMemory.Projectors
{
    public class InMemoryProjectionModelRepository : IProjectionModelRepository
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, IProjection>> _db = new ConcurrentDictionary<string, ConcurrentDictionary<string, IProjection>>();

        public Task Save(IProjection model, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var bag = _db.GetOrAdd(model.ProjectorId, s => new ConcurrentDictionary<string, IProjection>());
                bag.AddOrUpdate(model.Id, model, (id, existing) => model);
            }, cancellationToken);
        }

        public Task<TProjectionModel> Get<TProjectionModel>(string id, string projectorId, CancellationToken cancellationToken) where TProjectionModel : IProjection
        {
            return Task.Run(() =>
            {

                if (!_db.TryGetValue(projectorId, out var bag))
                {

                    return default(TProjectionModel);
                }

                if (!bag.TryGetValue(id, out IProjection projection))
                {
                    return default(TProjectionModel);
                }

                return (TProjectionModel)projection;
            }, cancellationToken);


        }

        public async Task List(string projectorId, Func<IProjection, CancellationToken, Task> callback, int skip, int take, CancellationToken cancellationToken)
        {
            if (!_db.TryGetValue(projectorId, out var bag))
            {
                return;
            }

            var query = bag.Values.AsQueryable();

            if (skip > 0)
            {
                query = query.Skip(skip);
            }

            if (take > 0)
            {
                query = query.Take(take);
            }

            foreach (var projection in bag.Values)
            {
                await callback(projection, cancellationToken);
            }
        }

        public Task List(string projectorId, Func<IProjection, CancellationToken, Task> callback, CancellationToken cancellationToken)
        {
            return List(projectorId, callback, -1, -1, cancellationToken);
        }
    }
}
