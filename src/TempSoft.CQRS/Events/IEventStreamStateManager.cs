using System.Threading.Tasks;

namespace TempSoft.CQRS.Events
{
    public interface IEventStreamStateManager
    {
        Task<EventStreamStatus> GetStatusForStream(string streamName);

        Task SetStatusForStream(string streamName, EventStreamStatus status);

        Task<int> GetEventCountForStream(string streamName);

        Task AddToEventCountForStream(string streamName, int count = 1);
    }
}