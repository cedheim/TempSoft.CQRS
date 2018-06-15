using System;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Entities
{
    public class Identifier : AggregateRoot<Movie>.Entity<Identifier>
    {
        public Identifier(Movie root, string id) : base(root, id)
        {
        }

        public string Value { get; private set; }

        [CommandHandler(typeof(SetIdentifier))]
        public void Set(string value)
        {
            ApplyChange(new IdentifierUpdated(value));
        }

        [EventHandler(typeof(IdentifierUpdated))]
        public void Updated(string value)
        {
            Value = value;
        }
    }
}