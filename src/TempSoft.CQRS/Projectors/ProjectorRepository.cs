using System;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Infrastructure;

namespace TempSoft.CQRS.Projectors
{
    public class ProjectorRepository : IProjectorRepository
    {
        private readonly IServiceProvider _serviceProvider;

        public ProjectorRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<IProjector> Get(Type type, string id, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var projector = (IProjector)_serviceProvider.GetService(type);
                projector.ProjectorId = id;

                return projector;
            }, cancellationToken);
        }

        public async Task<TProjector> Get<TProjector>(string id, CancellationToken cancellationToken) where TProjector : IProjector
        {
            return (TProjector) await Get(typeof(TProjector), id, cancellationToken);
        }
    }
}