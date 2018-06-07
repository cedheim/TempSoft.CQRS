using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Projectors
{
    public abstract class ProjectorBase : IProjector
    {
        public string ProjectorId { get; set; }
        public Task Project(IEvent @event, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IQueryResult> Query(IQuery query, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}