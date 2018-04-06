using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Demo.Domain.Theatres.Commands;
using TempSoft.CQRS.Demo.Domain.Theatres.Entities;
using TempSoft.CQRS.Demo.Domain.Theatres.Events;
using TempSoft.CQRS.Demo.Domain.Theatres.Exceptions;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.Demo.Tests.Domain.Theatres
{
    [TestFixture]
    public class When_adding_an_auditorium_that_already_exists
    {
        private Theatre _root;
        private Commit _commit;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _root = new Theatre();
            await _root.Initialize(Data.AggregateRootId, Data.TheatreName, CancellationToken.None);

            var command1 = new AddAuditorium(Data.AuditoriumId, Data.AuditoriumName);
            var command2 = new AddAuditorium(Data.AuditoriumId, Data.AuditoriumName);
            await _root.Handle(command1, CancellationToken.None);
            _root.Invoking(r => r.Handle(command2, CancellationToken.None).Wait()).Should().Throw<DuplicateAuditoriumAddedException>();

            _commit = _root.Commit();
        }

        [Test]
        public void Should_have_added_an_auditorium()
        {
            _root.Auditoriums.Should().ContainSingle(a => a.Id == Data.AuditoriumId && a.Name == Data.AuditoriumName);
        }

        [Test]
        public void Should_have_generated_event()
        {
            _commit.Events.Should().ContainSingle(e => e is AuditoriumAdded && ((AuditoriumAdded)e).AuditoriumId == Data.AuditoriumId && ((AuditoriumAdded)e).Name == Data.AuditoriumName);
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