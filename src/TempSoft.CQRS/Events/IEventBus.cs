using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.Events
{
    public interface IEventBus
    {
        Task Publish(IEnumerable<IEvent> events, CancellationToken cancellationToken);
    }
}