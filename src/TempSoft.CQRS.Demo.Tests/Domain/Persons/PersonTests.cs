using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Persons.Commands;
using TempSoft.CQRS.Demo.Domain.Persons.Entities;
using TempSoft.CQRS.Demo.Domain.Persons.Events;
using TempSoft.CQRS.Demo.Domain.Persons.Models;
using TempSoft.CQRS.Demo.ValueObjects;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Tests.Domain.Persons
{
    [TestFixture]
    public class PersonTests
    {
        private Person _entity;

        [SetUp]
        public async Task SetUp()
        {
            _entity = new Person() {Id = Data.AggregateRootId};
            await _entity.Handle(new CreatePerson(new Name(Data.FirstName, Data.LastName)), CancellationToken.None);
        }

        [Test]
        public void Should_have_created_a_person()
        {
            var commit = _entity.Commit();

            _entity.Name.Should().BeEquivalentTo(Data.Name);

            commit.Events.Should().ContainSingle(e => e is PersonCreated && ((PersonCreated) e).Name == Data.Name);
        }

        [Test]
        public void Should_be_able_to_get_the_read_model()
        {
            var model = (PersonModel)_entity.GetReadModel();

            model.Id.Should().Be(_entity.Id);
            model.Name.Should().BeEquivalentTo(_entity.Name);
            model.Version.Should().Be(_entity.Version);
        }

        [TestCase(typeof(PersonCreated), new object[] { Data.FirstName, default(string), Data.LastName })]
        public void Should_be_able_to_serialize_event(Type eventType, object[] arguments)
        {
            var o1 = Activator.CreateInstance(eventType, arguments) as IEvent;
            o1.AggregateRootId = Data.AggregateRootId;
            o1.EventGroup = nameof(Person);
            o1.Version = 1;

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(o1);
            var o2 = Newtonsoft.Json.JsonConvert.DeserializeObject(json, eventType);

            o2.Should().BeEquivalentTo(o1);
        }

        [TestCase(typeof(CreatePerson), new object[] { Data.FirstName, default(string), Data.LastName })]
        public void Should_be_able_to_serialize_command(Type commandType, object[] arguments)
        {
            var o1 = Activator.CreateInstance(commandType, arguments) as ICommand;
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(o1);
            var o2 = Newtonsoft.Json.JsonConvert.DeserializeObject(json, commandType);

            o2.Should().BeEquivalentTo(o1);
        }

        private static class Data
        {
            public static readonly Guid AggregateRootId = Guid.NewGuid();

            public const string FirstName = "Bruce";
            public const string LastName = "Willis";
            public static readonly Name Name = new Name(Data.FirstName, Data.LastName);
        }
    }
}