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
        private readonly ConcurrentDictionary<string, IProjection> _db = new ConcurrentDictionary<string, IProjection>();

        public Task Save<TProjectionModel>(TProjectionModel model, CancellationToken cancellationToken) where TProjectionModel : IProjection
        {
            return Task.Run(() =>
            {
                _db.AddOrUpdate(model.Id, model, (id, existing) => model);
            }, cancellationToken);
        }

        public Task<TProjectionModel> Get<TProjectionModel>(string id, CancellationToken cancellationToken) where TProjectionModel : IProjection
        {
            return Task.Run(() =>
            {
                if (_db.TryGetValue(id, out IProjection projection))
                {
                    return (TProjectionModel) projection;
                }

                return default(TProjectionModel);
            }, cancellationToken);


        }

        public async Task List(string projectorId, Func<IProjection, CancellationToken, Task> callback, CancellationToken cancellationToken)
        {
            foreach (var projection in _db.Values.Where(p => p.ProjectorId == projectorId))
            {
                await callback(projection, cancellationToken);
            }
        }
    }
}
