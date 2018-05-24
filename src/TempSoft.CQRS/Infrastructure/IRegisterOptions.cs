namespace TempSoft.CQRS.Infrastructure
{
    public interface IRegisterOptions
    {
        IRegisterOptions AsSingleton();
        IRegisterOptions AsMultiInstance();
        IRegisterOptions WithWeakReference();
        IRegisterOptions WithStrongReference();
    }
}