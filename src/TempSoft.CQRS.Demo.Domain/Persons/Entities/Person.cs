using System.Threading.Tasks;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Persons.Commands;
using TempSoft.CQRS.Demo.Domain.Persons.Events;
using TempSoft.CQRS.Demo.Domain.Persons.Models;
using TempSoft.CQRS.Demo.ValueObjects;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Persons.Entities
{
    public class Person : AggregateRoot<Person>, IAggregateRootWithReadModel
    {

        [CommandHandler(typeof(CreatePerson))]
        public void Create(Name name)
        {
            ApplyChange(new PersonCreated(name));
        }

        [EventHandler(typeof(PersonCreated))]
        public void Created(Name name)
        {
            Name = name;
        }

        public Name Name { get; private set; }

        public IAggregateRootReadModel GetReadModel()
        {
            return new PersonModel
            {
                Id = Id,
                Name = Name,
                Version = Version
            };
        }
    }
}