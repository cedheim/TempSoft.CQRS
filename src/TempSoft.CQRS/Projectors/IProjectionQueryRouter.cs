using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.Projectors
{
    public interface IProjectionQueryRouter
    {
        Task<TQueryResult> SendQuery<TProjector, TQueryResult>(IQuery query, string projectorId, CancellationToken cancellationToken) where TProjector : IProjector where TQueryResult : IQueryResult;
    }
}