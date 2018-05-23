using System;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Entities
{
    public class Slot : AggregateRoot<Theatre>.Entity<Slot>
    {
        public Slot(Theatre root, Guid id, string name, int order) : base(root, id)
        {
            Name = name;
            Order = order;
        }

        public string Name { get; }

        public int Order { get; }
    }
}