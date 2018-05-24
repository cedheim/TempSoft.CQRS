using TinyIoC;

namespace TempSoft.CQRS.Infrastructure
{
    public class MultiRegisterOptions : IMultiRegisterOptions
    {
        private readonly TinyIoCContainer.MultiRegisterOptions _multiRegisterOptions;

        internal MultiRegisterOptions(TinyIoCContainer.MultiRegisterOptions multiRegisterOptions)
        {
            _multiRegisterOptions = multiRegisterOptions;
        }

        public IMultiRegisterOptions AsSingleton()
        {
            return new MultiRegisterOptions(_multiRegisterOptions.AsSingleton());
        }

        public IMultiRegisterOptions AsMultiInstance()
        {
            return new MultiRegisterOptions(_multiRegisterOptions.AsMultiInstance());
        }
    }
}