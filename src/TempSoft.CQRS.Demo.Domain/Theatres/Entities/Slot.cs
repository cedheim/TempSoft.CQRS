using System;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Entities
{
    public class Slot : Theatre.Entity<Slot>
    {
        public Slot(Theatre root, Guid id, string name, int order) : base(root, id)
        {
            Name = name;
            Order = order;
        }

        public string Name { get; private set; }

        public int Order { get; private set; }
    }
}