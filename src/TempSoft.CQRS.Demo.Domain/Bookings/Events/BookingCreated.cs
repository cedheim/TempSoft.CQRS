using System;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Bookings.Events
{
    public class BookingCreated : InitializationEventBase
    {
        private BookingCreated() { }

        public BookingCreated(Guid aggregateRootId, Guid theatre, Guid auditorium, Guid slot, Guid movie, Guid movieVersion)
        {
            AggregateRootId = aggregateRootId;
            Theatre = theatre;
            Auditorium = auditorium;
            Slot = slot;
            Movie = movie;
            MovieVersion = movieVersion;
        }

        public Guid Theatre { get; private set; }
        public Guid Auditorium { get; private set; }
        public Guid Slot { get; private set; }
        public Guid Movie { get; private set; }
        public Guid MovieVersion { get; private set;  }
    }
}