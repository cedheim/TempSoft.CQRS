namespace TempSoft.CQRS.Demo.Common.Configuration
{
    public interface IApplicationSettings
    {
        string this[string key] { get; }
    }
}