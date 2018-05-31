using System.Collections.Generic;
using System.Fabric;

namespace TempSoft.CQRS.Demo.Configuration
{
    public class ApplicationSettings : IApplicationSettings
    {
        private readonly Dictionary<string, string> _internalDictionaryDictionary = new Dictionary<string, string>();

        public ApplicationSettings(ICodePackageActivationContext context)
        {
            Initialize(context);
        }

        private void Initialize(ICodePackageActivationContext context)
        {
            var sections = context.GetConfigurationPackageObject("Config")?.Settings?.Sections;
            if (sections == null)
                return;

            foreach (var section in sections)
            {
                var parameters = section?.Parameters;
                if (parameters == null)
                    continue;

                foreach (var parameter in parameters)
                {
                    if (_internalDictionaryDictionary.ContainsKey(parameter.Name))
                        continue;

                    _internalDictionaryDictionary.Add(parameter.Name, parameter.Value);
                }
            }
        }

        public string this[string key] => _internalDictionaryDictionary[key];
    }
}
 