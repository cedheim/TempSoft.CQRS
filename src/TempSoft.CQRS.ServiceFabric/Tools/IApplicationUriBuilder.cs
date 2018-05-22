using System.Fabric;

namespace TempSoft.CQRS.ServiceFabric.Tools
{
    public interface IApplicationUriBuilder
    {
        ICodePackageActivationContext ActivationContext { get; set; }
        string ApplicationInstance { get; set; }

        IServiceUriBuilder Build(string serviceInstance);
    }
}