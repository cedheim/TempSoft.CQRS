using System;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Models
{
    public class TheatreReadModel : IAggregateRootReadModel
    {
        public string Name { get; set; }

        public AuditoriumReadModel[] Auditoriums { get; set; }

        public SlotReadModel[] Slots { get; set; }
        public Guid Id { get; set; }

        public int Version { get; set; }
    }
}