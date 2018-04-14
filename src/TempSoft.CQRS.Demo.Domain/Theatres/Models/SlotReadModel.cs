using System;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Models
{
    public class SlotReadModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }
    }
}