using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Bookings;
using TempSoft.CQRS.Demo.Domain.Bookings.Commands;
using TempSoft.CQRS.Demo.Domain.Bookings.Entity;
using TempSoft.CQRS.Demo.Domain.Bookings.Events;
using TempSoft.CQRS.Demo.Domain.Bookings.Exceptions;
using TempSoft.CQRS.Demo.Domain.Movies.Entity;
using TempSoft.CQRS.Demo.Domain.Movies.Models;
using TempSoft.CQRS.Demo.Domain.Theatres.Entities;
using TempSoft.CQRS.Demo.Domain.Theatres.Models;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Infrastructure;

namespace TempSoft.CQRS.Demo.Tests.Domain
{
    [TestFixture]
    public class Bookings
    {
        private ICommandRouter _fakeRouter;
        private MovieReadModel _movie;
        private TheatreReadModel _theatre;

        [SetUp]
        public async Task SetUp()
        {
            _movie = new MovieReadModel
            {
                Id = Data.MovieId,
                Versions = new[]
                {
                    new VersionReadModel
                    {
                        Id = Data.MovieVersion
                    }
                }
            };
            _theatre = new TheatreReadModel
            {
                Id = Data.Theatre,
                Auditoriums = new[]
                {
                    new AuditoriumReadModel
                    {
                        Id = Data.Auditorium
                    }
                },
                Slots = new[]
                {
                    new SlotReadModel
                    {
                        Id = Data.Slot
                    }
                }
            };

            _fakeRouter = A.Fake<ICommandRouter>();

            A.CallTo(() => _fakeRouter.GetReadModel<Movie, MovieReadModel>(Data.MovieId, A<CancellationToken>.Ignored))
                .ReturnsLazily(foc => Task.Run(() => _movie));
            A.CallTo(() => _fakeRouter.GetReadModel<Theatre, TheatreReadModel>(Data.Theatre, A<CancellationToken>.Ignored))
                .ReturnsLazily(foc => Task.Run(() => _theatre));
        }

        [Test]
        public async Task When_creating_a_new_booking()
        {

            var root = new Booking(_fakeRouter);
            await root.Handle(new CreateBooking(Data.AggregateRootId, Data.Theatre, Data.Auditorium, Data.Slot, Data.MovieId, Data.MovieVersion), CancellationToken.None);

            var commit = root.Commit();

            root.Auditorium.Should().Be(Data.Auditorium);
            root.Movie.Should().Be(Data.MovieId);
            root.MovieVersion.Should().Be(Data.MovieVersion);
            root.Theatre.Should().Be(Data.Theatre);
            root.Slot.Should().Be(Data.Slot);

            commit.Events.Should().ContainSingle(e => e is BookingCreated &&
                ((BookingCreated) e).Auditorium == Data.Auditorium &&
                ((BookingCreated) e).Movie == Data.MovieId &&
                ((BookingCreated) e).Theatre == Data.Theatre &&
                ((BookingCreated) e).Slot == Data.Slot &&
                ((BookingCreated) e).MovieVersion == Data.MovieVersion);

            A.CallTo(() => _fakeRouter.GetReadModel<Movie, MovieReadModel>(Data.MovieId, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _fakeRouter.GetReadModel<Theatre, TheatreReadModel>(Data.Theatre, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void When_creating_a_new_booking_and_the_movie_version_does_not_exist()
        {
            var root = new Booking(_fakeRouter);
            root.Invoking(r => r.Handle(new CreateBooking(Data.AggregateRootId, Data.Theatre, Data.Auditorium, Data.Slot, Data.MovieId, Guid.NewGuid()), CancellationToken.None).Wait())
                .Should().Throw<MovieVersionMissingException>();
        }

        [Test]
        public void When_creating_a_new_booking_and_the_auditorium_does_not_exist()
        {
            var root = new Booking(_fakeRouter);
            root.Invoking(r => r.Handle(new CreateBooking(Data.AggregateRootId, Data.Theatre, Guid.NewGuid(), Data.Slot, Data.MovieId, Data.MovieVersion), CancellationToken.None).Wait())
                .Should().Throw<AuditoriumMissingException>();
        }

        [Test]
        public void When_creating_a_new_booking_and_the_slot_does_not_exist()
        {
            var root = new Booking(_fakeRouter);
            root.Invoking(r => r.Handle(new CreateBooking(Data.AggregateRootId, Data.Theatre, Data.Auditorium, Guid.NewGuid(), Data.MovieId, Data.MovieVersion), CancellationToken.None).Wait())
                .Should().Throw<SlotMissingException>();
        }

        private static class Data
        {
            public static readonly Guid AggregateRootId = Guid.NewGuid();
            public static readonly Guid Theatre = Guid.NewGuid();
            public static readonly Guid Auditorium = Guid.NewGuid();
            public static readonly Guid MovieId = Guid.NewGuid();
            public static readonly Guid Slot = Guid.NewGuid();
            public static readonly Guid MovieVersion = Guid.NewGuid();
        }
    }
}
