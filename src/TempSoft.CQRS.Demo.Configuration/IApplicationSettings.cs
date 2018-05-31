namespace TempSoft.CQRS.Demo.Configuration
{
    public interface IApplicationSettings
    {
        string this[string key] { get; }
    }
}