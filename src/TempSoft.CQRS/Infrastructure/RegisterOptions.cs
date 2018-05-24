using TinyIoC;

namespace TempSoft.CQRS.Infrastructure
{
    public class RegisterOptions : IRegisterOptions
    {
        private readonly TinyIoCContainer.RegisterOptions _registerOptions;

        internal RegisterOptions(TinyIoCContainer.RegisterOptions registerOptions)
        {
            _registerOptions = registerOptions;
        }

        public IRegisterOptions AsSingleton()
        {
            return new RegisterOptions(_registerOptions.AsSingleton());
        }

        public IRegisterOptions AsMultiInstance()
        {
            return new RegisterOptions(_registerOptions.AsMultiInstance());
        }

        public IRegisterOptions WithWeakReference()
        {
            return new RegisterOptions(_registerOptions.WithWeakReference());
        }

        public IRegisterOptions WithStrongReference()
        {
            return new RegisterOptions(_registerOptions.WithStrongReference());
        }
    }
}