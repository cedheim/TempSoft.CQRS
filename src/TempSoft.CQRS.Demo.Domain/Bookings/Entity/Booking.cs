using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Bookings.Commands;
using TempSoft.CQRS.Demo.Domain.Bookings.Events;
using TempSoft.CQRS.Demo.Domain.Bookings.Exceptions;
using TempSoft.CQRS.Demo.Domain.Movies.Entity;
using TempSoft.CQRS.Demo.Domain.Movies.Models;
using TempSoft.CQRS.Demo.Domain.Theatres.Entities;
using TempSoft.CQRS.Demo.Domain.Theatres.Models;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Bookings.Entity
{
    public class Booking : AggregateRoot<Booking>
    {
        private readonly ICommandRouter _router;

        public Booking(ICommandRouter router)
        {
            _router = router;
        }

        public Guid Theatre { get; private set; }
        public Guid Auditorium { get; private set; }
        public Guid Slot { get; private set; }
        public Guid Movie { get; private set; }
        public Guid MovieVersion { get; private set; }

        [CommandHandler(typeof(CreateBooking))]
        public async Task Initialize(Guid aggregateRootId, Guid theatre, Guid auditorium, Guid slot, Guid movie,
            Guid movieVersion, CancellationToken cancellationToken)
        {
            var movieModelTask = _router.GetReadModel<Movie, MovieReadModel>(movie, cancellationToken);
            var theatreModelTask = _router.GetReadModel<Theatre, TheatreReadModel>(theatre, cancellationToken);
            await Task.WhenAll(movieModelTask, theatreModelTask);

            var movieModel = movieModelTask.Result;
            var theatreModel = theatreModelTask.Result;

            var movieVersionModel = movieModel.Versions.FirstOrDefault(v => v.Id == movieVersion);
            if (ReferenceEquals(movieVersionModel, default(VersionReadModel)))
                throw new MovieVersionMissingException(
                    $"Could not find movie version {movieVersion} on movie {movie}.");

            var auditoriumModel = theatreModel.Auditoriums.FirstOrDefault(a => a.Id == auditorium);
            if (ReferenceEquals(auditoriumModel, default(AuditoriumReadModel)))
                throw new AuditoriumMissingException($"Could not find auditorium {auditorium} for theatre {theatre}.");

            var slotModel = theatreModel.Slots.FirstOrDefault(s => s.Id == slot);
            if (ReferenceEquals(slotModel, default(SlotReadModel)))
                throw new SlotMissingException($"Could not find slot {slot} for theatre {theatre}.");

            ApplyChange(new BookingCreated(aggregateRootId, theatre, auditorium, slot, movie, movieVersion));
        }

        [EventHandler(typeof(BookingCreated))]
        private void Apply(BookingCreated @event)
        {
            Theatre = @event.Theatre;
            Auditorium = @event.Auditorium;
            Slot = @event.Slot;
            Movie = @event.Movie;
            MovieVersion = @event.MovieVersion;
        }
    }
}