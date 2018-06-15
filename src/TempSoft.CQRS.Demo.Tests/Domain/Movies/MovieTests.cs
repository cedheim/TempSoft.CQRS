using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators.Emitters;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Entities;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Demo.Domain.Movies.Exceptions;
using TempSoft.CQRS.Demo.Domain.Movies.Models;
using TempSoft.CQRS.Demo.Domain.Persons.Entities;
using TempSoft.CQRS.Demo.Domain.Persons.Models;
using TempSoft.CQRS.Demo.ValueObjects;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Tests.Domain.Movies
{
    [TestFixture]
    public class MovieTests
    {
        private Movie _entity;
        private ICommandRouter _router;

        [SetUp]
        public async Task SetUp()
        {
            _router = A.Fake<ICommandRouter>();

            _entity = new Movie(_router) {Id = Data.AggregateRootId};
            await _entity.Handle(new CreateMovie(Data.OriginalTitle), CancellationToken.None);
        }

        [Test]
        public void Should_be_able_to_create_a_new_movie()
        {
            var commit = _entity.Commit();

            commit.Events.Should().ContainSingle(e => e.AggregateRootId == Data.AggregateRootId && e is MovieCreated && ((MovieCreated)e).OriginalTitle == Data.OriginalTitle);
            _entity.Id.Should().Be(Data.AggregateRootId);
            _entity.OriginalTitle.Should().Be(Data.OriginalTitle);
        }

        [Test]
        public async Task Should_be_able_to_set_local_title()
        {
            await _entity.Handle(new SetLocalTitle(Data.Culture, Data.LocalTitle), CancellationToken.None);
            var commit = _entity.Commit();

            commit.Events.Should().ContainSingle(e => e is LocalInformationCreated && ((LocalInformationCreated)e).Culture == Data.Culture);
            commit.Events.Should().ContainSingle(e => e is LocalTitleSet && ((LocalTitleSet)e).Title == Data.LocalTitle);

            var localInformation = _entity.LocalInformation.FirstOrDefault(li => li.Culture == Data.Culture);
            localInformation.Culture.Should().BeEquivalentTo(Data.Culture);
            localInformation.Title.Should().BeEquivalentTo(Data.LocalTitle);
        }

        [Test]
        public async Task Should_be_able_to_get_a_read_model()
        {
            await _entity.Handle(new SetLocalTitle(Data.Culture, Data.LocalTitle), CancellationToken.None);
            await _entity.Handle(new SetIdentifier(Data.IdentifierId, Data.IdentifierValue), CancellationToken.None);

            var model = (MovieModel) _entity.GetReadModel();

            model.Id.Should().Be(_entity.Id);
            model.Version.Should().Be(_entity.Version);
            model.OriginalTitle.Should().Be(_entity.OriginalTitle);
            model.LocalInformation.Should().ContainKey(Data.Culture.ToString());
            model.Identifiers.Should().Contain(Data.IdentifierId, Data.IdentifierValue);
        }

        [Test]
        public async Task Should_be_able_to_set_an_external_identifier()
        {
            await _entity.Handle(new SetIdentifier(Data.IdentifierId, Data.IdentifierValue), CancellationToken.None);
            var commit = _entity.Commit();

            _entity.Identifiers.Should().ContainSingle(id => id.Id == Data.IdentifierId && id.Value == Data.IdentifierValue);
            
            commit.Events.Should().ContainSingle(e => e is IdentifierCreated && ((IdentifierCreated)e).IdentifierId == Data.IdentifierId);
            commit.Events.Should().ContainSingle(e => e is IdentifierUpdated && ((IdentifierUpdated)e).Value == Data.IdentifierValue);
        }

        [TestCase(typeof(MovieCreated), new object[]{ Data.OriginalTitle })]
        [TestCase(typeof(IdentifierCreated), new object[] { Data.IdentifierId })]
        [TestCase(typeof(IdentifierUpdated), new object[] { Data.IdentifierValue })]
        [TestCase(typeof(LocalTitleSet), new object[] { Data.LocalTitle })]
        [TestCase(typeof(LocalInformationCreated), new object[] { Data.Country, Data.Language })]
        public void Should_be_able_to_serialize_event(Type eventType, object[] arguments)
        {
            var o1 = Activator.CreateInstance(eventType, arguments) as IEvent;
            o1.AggregateRootId = Data.AggregateRootId;
            o1.EventGroup = nameof(Movie);
            o1.Version = 1;

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(o1);
            var o2 = Newtonsoft.Json.JsonConvert.DeserializeObject(json, eventType);

            o2.Should().BeEquivalentTo(o1);
        }

        [TestCase(typeof(CreateMovie), new object[]{ Data.OriginalTitle })]
        [TestCase(typeof(SetIdentifier), new object[] { Data.IdentifierId, Data.IdentifierValue })]
        [TestCase(typeof(SetLocalTitle), new object[] { Data.Country, Data.Language, Data.LocalTitle })]
        [TestCase(typeof(AddActor), new object[] { Data.ActorIdString })]
        public void Should_be_able_to_serialize_command(Type commandType, object[] arguments)
        {
            var o1 = Activator.CreateInstance(commandType, arguments) as ICommand;
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(o1);
            var o2 = Newtonsoft.Json.JsonConvert.DeserializeObject(json, commandType);

            o2.Should().BeEquivalentTo(o1);
        }

        [Test]
        public async Task Should_be_able_to_add_a_person_as_an_actor()
        {
            A.CallTo(() => _router.GetReadModel<Person, PersonModel>(A<Guid>.Ignored, A<CancellationToken>.Ignored))
                .Returns(new PersonModel {Id = Data.ActorId});

            await _entity.Handle(new AddActor(Data.ActorId), CancellationToken.None);
            var commit = _entity.Commit();

            commit.Events.Should().ContainSingle(e => e is PersonAdded && ((PersonAdded)e).PersonId == Data.ActorId && ((PersonAdded)e).Role == "ACTOR");

            A.CallTo(() => _router.GetReadModel<Person, PersonModel>(Data.ActorId, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);

            var persons = _entity.Persons;

            persons.Should().ContainKey("ACTOR");
            persons["ACTOR"].Should().ContainSingle(personId => personId == Data.ActorId);
        }

        [Test]
        public void Should_not_be_able_to_add_a_person_if_it_does_not_exist()
        {
            A.CallTo(() => _router.GetReadModel<Person, PersonModel>(A<Guid>.Ignored, A<CancellationToken>.Ignored))
                .Returns(default(PersonModel));

            _entity.Invoking(e => e.Handle(new AddActor(Data.ActorId), CancellationToken.None).Wait())
                .Should().Throw<UnableToAddPersonException>();
        }

        private static class Data
        {
            public const string Country = "SE";

            public const string Language = "sv";

            public static readonly Guid AggregateRootId = Guid.NewGuid();

            public const string OriginalTitle = "Star Wars IV: A New Hope";

            public const string LocalTitle = "Stjärnornas Krig";

            public static readonly Culture Culture = new Culture(Data.Country, Data.Language);

            public const string IdentifierId = "IMDB";

            public const string IdentifierValue = "tt0076759";

            public const string ActorIdString = "aecb0185-401f-419f-8991-f0c56ea5e202";

            public static readonly Guid ActorId = Guid.Parse(ActorIdString);
        }
    }
}
