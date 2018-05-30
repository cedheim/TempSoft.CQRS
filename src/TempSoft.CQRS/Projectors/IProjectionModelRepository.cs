using System;
using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.Projectors
{
    public interface IProjectionModelRepository
    {
        Task Save(IProjection model, CancellationToken cancellationToken);

        Task<TProjectionModel> Get<TProjectionModel>(string id, string projectorId, CancellationToken cancellationToken) where TProjectionModel : IProjection;

        Task List(string projectorId, Func<IProjection, CancellationToken, Task> callback, CancellationToken cancellationToken);
    }
}