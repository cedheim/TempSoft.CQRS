using TinyIoC;

namespace TempSoft.CQRS.Infrastructure
{
    public class RegisterOptions
    {
        private readonly TinyIoCContainer.RegisterOptions _registerOptions;

        internal RegisterOptions(TinyIoC.TinyIoCContainer.RegisterOptions registerOptions)
        {
            _registerOptions = registerOptions;
        }

        public RegisterOptions AsSingleton()
        {
            return new RegisterOptions(_registerOptions.AsSingleton());
        }

        public RegisterOptions AsMultiInstance()
        {
            return new RegisterOptions(_registerOptions.AsMultiInstance());
        }

        public RegisterOptions WithWeakReference()
        {
            return new RegisterOptions(_registerOptions.WithWeakReference());
        }

        public RegisterOptions WithStrongReference()
        {
            return new RegisterOptions(_registerOptions.WithStrongReference());
        }
    }
}