using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Entities;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Demo.Domain.Movies.Models;
using TempSoft.CQRS.Demo.Domain.Movies.Values;

namespace TempSoft.CQRS.Demo.Tests.Movies
{
    [TestFixture]
    public class MovieTests
    {
        private Movie _entity;

        [SetUp]
        public async Task SetUp()
        {
            _entity = new Movie() {Id = Data.AggregateRootId};
            await _entity.Handle(new CreateMovie(Data.OriginalTitle), CancellationToken.None);
        }

        [Test]
        public void When_creating_a_new_movie()
        {
            var commit = _entity.Commit();

            commit.Events.Should().ContainSingle(e => e.AggregateRootId == Data.AggregateRootId && e is MovieCreated && ((MovieCreated)e).OriginalTitle == Data.OriginalTitle);
            _entity.Id.Should().Be(Data.AggregateRootId);
            _entity.OriginalTitle.Should().Be(Data.OriginalTitle);
        }

        [Test]
        public async Task When_setting_a_local_title()
        {
            await _entity.Handle(new SetLocalTitle(Data.Country, Data.LocalTitle), CancellationToken.None);
            var commit = _entity.Commit();

            commit.Events.Should().ContainSingle(e => e is LocalInformationCreated && ((LocalInformationCreated)e).Country == Data.Country);
            commit.Events.Should().ContainSingle(e => e is LocalTitleSet && ((LocalTitleSet)e).Title == Data.LocalTitle);

            var localInformation = _entity.LocalInformation.FirstOrDefault(li => li.Country == Data.Country);
            localInformation.Country.Should().BeEquivalentTo(Data.Country);
            localInformation.Title.Should().BeEquivalentTo(Data.LocalTitle);
        }

        [Test]
        public async Task When_getting_a_read_model()
        {
            await _entity.Handle(new SetLocalTitle(Data.Country, Data.LocalTitle), CancellationToken.None);

            var model = (MovieModel)_entity.GetReadModel();

            model.Id.Should().Be(_entity.Id);
            model.Version.Should().Be(_entity.Version);
            model.OriginalTitle.Should().Be(_entity.OriginalTitle);
            model.LocalInformation.Should().ContainSingle(li => model.LocalInformation.Any(li2 => li2.Country == li.Country && li2.Title == li.Title));

        }


        
        private static class Data
        {
            public static readonly Guid AggregateRootId = Guid.NewGuid();

            public const string OriginalTitle = "Star Wars IV: A New Hope";

            public const string LocalTitle = "Stjärnornas Krig";

            public static readonly Country Country = new Country("se");
        }
    }
}