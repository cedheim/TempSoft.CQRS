namespace TempSoft.CQRS.ServiceFabric.Tools
{
    public interface IUriHelper
    {
        System.Uri GetUriFor<TService>();
    }
}