using System;
using TempSoft.CQRS.Demo.ValueObjects;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.Demo.Domain.Persons.Models
{
    public class PersonModel : IAggregateRootReadModel
    {
        public Guid Id { get; set; }

        public int Version { get; set; }

        public Name Name { get; set; }
    }
}