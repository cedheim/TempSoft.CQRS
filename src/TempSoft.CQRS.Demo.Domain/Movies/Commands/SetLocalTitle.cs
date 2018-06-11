using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Values;

namespace TempSoft.CQRS.Demo.Domain.Movies.Commands
{
    public class SetLocalTitle : CommandBase
    {
        private SetLocalTitle()
        {
        }

        public SetLocalTitle(Country country, string title)
        {
            Country = country;
            Title = title;
        }

        public Country Country { get; private set; }

        public string Title { get; private set; }
    }
}