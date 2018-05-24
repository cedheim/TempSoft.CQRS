namespace TempSoft.CQRS.Infrastructure
{
    public interface IMultiRegisterOptions
    {
        IMultiRegisterOptions AsSingleton();
        IMultiRegisterOptions AsMultiInstance();
    }
}