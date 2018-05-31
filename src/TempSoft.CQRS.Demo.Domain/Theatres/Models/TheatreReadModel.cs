using System;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Models
{
    public class TheatreReadModel : IAggregateRootReadModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Version { get; set; }

        public AuditoriumReadModel[]  Auditoriums { get; set; }

        public SlotReadModel[] Slots { get; set; }
    }
}