using System;
using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.Projectors
{
    public interface IProjectorRepository
    {
        Task<IProjector> Get(Type type, string id, CancellationToken cancellationToken);

        Task<TProjector> Get<TProjector>(string id, CancellationToken cancellationToken) where TProjector : IProjector;
    }
}