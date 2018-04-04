using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace TempSoft.CQRS.Tests.Mocks
{
    public static class ServiceExtensions
    {
        public static async Task InvokeRunAsync(this IService o, CancellationToken cancellationToken)
        {
            var task = (Task)o.CallPrivateMethod("RunAsync", cancellationToken);

            await task;
        }
    }
}