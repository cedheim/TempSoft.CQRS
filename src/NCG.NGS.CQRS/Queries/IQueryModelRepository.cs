using System;
using System.Threading;
using System.Threading.Tasks;

namespace NCG.NGS.CQRS.Queries
{
    public interface IQueryModelRepository
    {
        Task<T> Get<T>(string id, CancellationToken cancellationToken);

        Task Save<T>(string id, T model, CancellationToken cancellationToken);
    }
}