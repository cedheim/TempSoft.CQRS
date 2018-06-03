namespace TempSoft.CQRS.Demo.SF.Configuration
{
    public interface IApplicationSettings
    {
        string this[string key] { get; }
    }
}