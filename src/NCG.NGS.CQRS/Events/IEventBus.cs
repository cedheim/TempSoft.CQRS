using System.Collections.Generic;
using System.Threading.Tasks;
using NCG.NGS.CQRS.Queries;

namespace NCG.NGS.CQRS.Events
{
    public interface IEventBus
    {
        Task Publish(IEnumerable<IEvent> events);
    }
}