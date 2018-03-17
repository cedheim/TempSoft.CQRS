using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCG.NGS.CQRS.Events
{
    public interface IEventBus
    {
        Task Publish(IEnumerable<IEvent> events);
    }
}