using System.Threading.Tasks;

namespace TempSoft.CQRS.Events
{
    public interface IEventStreamFactory
    {
        Task<IEventStream> Open(string name);
    }
}