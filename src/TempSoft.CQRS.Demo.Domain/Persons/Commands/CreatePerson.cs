using Newtonsoft.Json;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.ValueObjects;

namespace TempSoft.CQRS.Demo.Domain.Persons.Commands
{
    public class CreatePerson : CommandBase
    {
        public CreatePerson(string firstName, string middleName, string lastName)
        {
            Name = new Name(firstName, middleName, lastName);
        }

        [JsonConstructor]
        public CreatePerson(Name name)
        {
            Name = name;
        }

        public Name Name { get; private set; }
    }
}