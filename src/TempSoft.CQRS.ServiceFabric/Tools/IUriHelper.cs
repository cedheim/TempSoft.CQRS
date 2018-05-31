namespace TempSoft.CQRS.ServiceFabric.Tools
{
    public interface IUriHelper
    {
        void RegisterUri<TService>(System.Uri uri);

        System.Uri GetUriFor<TService>();
    }
}