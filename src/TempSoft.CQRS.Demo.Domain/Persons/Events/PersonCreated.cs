using Newtonsoft.Json;
using TempSoft.CQRS.Demo.ValueObjects;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Persons.Events
{
    public class PersonCreated : EventBase, IInitializationEvent
    {
        public PersonCreated(string firstName, string middleName, string lastName)
        {
            Name = new Name(firstName, middleName, lastName);
        }

        [JsonConstructor]
        public PersonCreated(Name name)
        {
            Name = name;
        }

        public Name Name { get; private set; }
    }
}