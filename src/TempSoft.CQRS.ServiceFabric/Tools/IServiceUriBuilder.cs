using System;
using System.Fabric;

namespace TempSoft.CQRS.ServiceFabric.Tools
{
    public interface IServiceUriBuilder
    {
        ICodePackageActivationContext ActivationContext { get; }
        string ApplicationInstance { get; }
        string ServiceInstance { get; }

        Uri ToUri();
    }
}