using TinyIoC;

namespace TempSoft.CQRS.Infrastructure
{
    public class MultiRegisterOptions
    {
        private readonly TinyIoCContainer.MultiRegisterOptions _multiRegisterOptions;

        internal MultiRegisterOptions(TinyIoCContainer.MultiRegisterOptions multiRegisterOptions)
        {
            _multiRegisterOptions = multiRegisterOptions;
        }

        public MultiRegisterOptions AsSingleton()
        {
            return new MultiRegisterOptions(_multiRegisterOptions.AsSingleton());
        }

        public MultiRegisterOptions AsMultiInstance()
        {
            return new MultiRegisterOptions(_multiRegisterOptions.AsMultiInstance());
        }
    }
}