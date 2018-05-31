using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Projectors
{
    public interface IProjector
    {
        string ProjectorId { get; set; }

        Task Project(IEvent @event, CancellationToken cancellationToken);
    }
}
