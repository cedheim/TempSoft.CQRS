using System;
using TempSoft.CQRS.Commands;

namespace TempSoft.CQRS.Demo.Domain.Bookings.Commands
{
    public class CreateBooking : CommandBase
    {
        private CreateBooking() { }

        public CreateBooking(Guid aggregateRootId, Guid theatre, Guid auditorium, Guid slot, Guid movie, Guid movieVersion)
        {
            AggregateRootId = aggregateRootId;
            Theatre = theatre;
            Auditorium = auditorium;
            Slot = slot;
            Movie = movie;
            MovieVersion = movieVersion;
        }
        public Guid AggregateRootId { get; private set; }
        public Guid Theatre { get; private set; }
        public Guid Auditorium { get; private set; }
        public Guid Slot { get; private set; }
        public Guid Movie { get; private set; }
        public Guid MovieVersion { get; private set; }
    }
}