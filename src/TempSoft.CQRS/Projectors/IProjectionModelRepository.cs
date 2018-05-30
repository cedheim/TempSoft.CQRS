using System;
using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.Projectors
{
    public interface IProjectionModelRepository
    {
        Task Save<TProjectionModel>(TProjectionModel model, CancellationToken cancellationToken) where TProjectionModel : IProjection;

        Task<TProjectionModel> Get<TProjectionModel>(string id, CancellationToken cancellationToken) where TProjectionModel : IProjection;

        Task List(string projectorId, Func<IProjection, CancellationToken, Task> callback, CancellationToken cancellationToken);
    }
}