namespace TempSoft.CQRS.Common.Uri
{
    public interface IUriHelper
    {
        System.Uri GetUriForSerivce<TService>();
    }
}