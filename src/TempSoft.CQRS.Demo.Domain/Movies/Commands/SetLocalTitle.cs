using Newtonsoft.Json;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.ValueObjects;

namespace TempSoft.CQRS.Demo.Domain.Movies.Commands
{
    public class SetLocalTitle : EntityCommandBase, ILocalInformationCommand
    {
        public SetLocalTitle(string country, string language, string title)
        {
            Culture = new Culture(country, language);
            EntityId = Culture.ToString();
            Title = title;
        }

        [JsonConstructor]
        public SetLocalTitle(Culture culture, string title)
        {
            EntityId = culture.ToString();
            Culture = culture;
            Title = title;
        }

        public Culture Culture { get; private set; }

        public string Title { get; private set; }
    }
}