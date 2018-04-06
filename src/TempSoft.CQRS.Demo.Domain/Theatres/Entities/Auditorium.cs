using System;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Entities
{
    public class Auditorium : Theatre.Entity<Auditorium>
    {
        public Auditorium(Theatre root, Guid id, string name)
            : base(root, id)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}