using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.InMemory.Projectors
{
    public class InMemoryProjectionQueryRouter : IProjectionQueryRouter
    {
        private readonly IProjectorRepository _projectorRepository;

        public InMemoryProjectionQueryRouter(IProjectorRepository projectorRepository)
        {
            _projectorRepository = projectorRepository;
        }

        public async Task<TQueryResult> SendQuery<TProjector, TQueryResult>(IQuery query, string projectorId, CancellationToken cancellationToken) where TProjector : IProjector where TQueryResult : IQueryResult
        {
            var projector = await _projectorRepository.Get<TProjector>(projectorId, cancellationToken);
            var result = await projector.Query(query, cancellationToken);

            return (TQueryResult) result;
        }
    }
}