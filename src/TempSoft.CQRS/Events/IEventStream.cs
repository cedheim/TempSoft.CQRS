using System;
using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.Events
{
    public interface IEventStream
    {
        Task<IEvent> Read(TimeSpan timeout, CancellationToken cancellationToken);
    }
}