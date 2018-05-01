using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.Events
{
    public interface IEventStream
    {
        Task Write(IEvent @event, CancellationToken cancellationToken);
    }
}