using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Demo.Domain.Theatres.Commands;
using TempSoft.CQRS.Demo.Domain.Theatres.Entities;
using TempSoft.CQRS.Demo.Domain.Theatres.Enums;
using TempSoft.CQRS.Demo.Domain.Theatres.Events;
using TempSoft.CQRS.Demo.Domain.Theatres.Exceptions;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.Demo.Tests.Domain
{
    [TestFixture]
    public class Theatres
    {
        private Theatre _root;
        private Commit _commit;
        private Auditorium _auditorium;

        [SetUp]
        public async Task SetUp()
        {
            _root = new Theatre();
            await _root.Handle(new InitializeTheatre(Data.AggregateRootId, Data.TheatreName), CancellationToken.None);
            await _root.Handle(new AddAuditorium(Data.AuditoriumId, Data.AuditoriumName), CancellationToken.None);

            _auditorium = _root.Auditoriums.FirstOrDefault();

            _commit = _root.Commit();
        }

        [Test]
        public void When_initializing_a_theatre()
        {
            _commit.Events.Should().ContainSingle(e => (e is TheatreInitialized) && ((TheatreInitialized)e).Name == Data.TheatreName && ((TheatreInitialized)e).AggregateRootId == Data.AggregateRootId);
            _root.Name.Should().Be(Data.TheatreName);
            _root.Id.Should().Be(Data.AggregateRootId);
        }

        [Test]
        public void When_adding_an_auditorium()
        {
            _commit.Events.Should().ContainSingle(e => e is AuditoriumAdded && ((AuditoriumAdded)e).AuditoriumId == Data.AuditoriumId && ((AuditoriumAdded)e).Name == Data.AuditoriumName);
            _auditorium.Name.Should().Be(Data.AuditoriumName);
            _auditorium.Id.Should().Be(Data.AuditoriumId);
        }

        [Test]
        public void When_adding_an_auditorium_that_already_exists()
        {
            _root.Invoking(r => r.Handle(new AddAuditorium(Data.AuditoriumId, Data.AuditoriumName), CancellationToken.None).Wait()).Should().Throw<DuplicateAuditoriumAddedException>();
        }

        [Test]
        public async Task When_adding_a_movie_property()
        {
            await _root.Handle(new AddAuditoriumProperty(Data.AuditoriumId, AuditoriumProperties.Is3D), CancellationToken.None);
            _auditorium.Is3D.Should().BeTrue();
        }

        [Test]
        public async Task When_adding_the_same_property_twice()
        {
            await _root.Handle(new AddAuditoriumProperty(Data.AuditoriumId, AuditoriumProperties.Is3D), CancellationToken.None);
            _root.Invoking(r => r.Handle(new AddAuditoriumProperty(Data.AuditoriumId, AuditoriumProperties.Is3D), CancellationToken.None).Wait())
                .Should().Throw<AuditoriumPropertyException>();
        }

        [Test]
        public void When_getting_the_read_model()
        {
            var readModel = _root.GetReadModel();
            readModel.Should().BeEquivalentTo(_root);
        }


        [Test]
        public async Task When_removing_property()
        {
            await _root.Handle(new AddAuditoriumProperty(Data.AuditoriumId, AuditoriumProperties.Is3D), CancellationToken.None);
            await _root.Handle(new RemoveAuditoriumProperty(Data.AuditoriumId, AuditoriumProperties.Is3D), CancellationToken.None);
            _auditorium.Is3D.Should().BeFalse();
        }

        [Test]
        public void When_removing_property_that_does_not_exist()
        {
            _root.Invoking(r => r.Handle(new RemoveAuditoriumProperty(Data.AuditoriumId, AuditoriumProperties.Is3D), CancellationToken.None).Wait())
                .Should().Throw<AuditoriumPropertyException>();
        }



        private static class Data
        {

            public static readonly Guid AggregateRootId = Guid.NewGuid();

            public const string TheatreName = "THEATRE1";

            public const string AuditoriumName = "AUDITORIUM1";

            public static readonly Guid AuditoriumId = Guid.NewGuid();
        }
    }
}