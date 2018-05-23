using System;
using TempSoft.CQRS.Commands;

namespace TempSoft.CQRS.Demo.Domain.Bookings.Commands
{
    public class CreateBooking : CommandBase
    {
        private CreateBooking()
        {
        }

        public CreateBooking(Guid aggregateRootId, Guid theatre, Guid auditorium, Guid slot, Guid movie,
            Guid movieVersion)
        {
            AggregateRootId = aggregateRootId;
            Theatre = theatre;
            Auditorium = auditorium;
            Slot = slot;
            Movie = movie;
            MovieVersion = movieVersion;
        }

        public Guid AggregateRootId { get; }
        public Guid Theatre { get; }
        public Guid Auditorium { get; }
        public Guid Slot { get; }
        public Guid Movie { get; }
        public Guid MovieVersion { get; }
    }
}