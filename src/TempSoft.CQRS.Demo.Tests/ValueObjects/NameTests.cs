using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using TempSoft.CQRS.Demo.ValueObjects;

namespace TempSoft.CQRS.Demo.Tests.ValueObjects
{
    [TestFixture]
    public class NameTests
    {
        [Test]
        public void Should_be_able_to_create_a_name()
        {
            var name = new Name(Data.FirstName, Data.MiddleName, Data.LastName);
            name.FirstName.Should().Be(Data.FirstName);
            name.MiddleName.Should().Be(Data.MiddleName);
            name.LastName.Should().Be(Data.LastName);
        }

        [Test]
        public void Should_be_able_to_create_a_name_without_a_middle_name()
        {
            var name = new Name(Data.FirstName, Data.LastName);
            name.FirstName.Should().Be(Data.FirstName);
            name.LastName.Should().Be(Data.LastName);
        }

        [Test]
        public void Should_be_able_to_compare_two_names()
        {
            var name1 = new Name(Data.FirstName, Data.MiddleName, Data.LastName);
            var name2 = new Name(Data.FirstName, Data.MiddleName, Data.LastName);

            (name1 == name2).Should().BeTrue();
            (name1 != name2).Should().BeFalse();
        }

        [Test]
        public void Should_be_able_to_serialize_a_name()
        {
            var name1 = new Name(Data.FirstName, Data.MiddleName, Data.LastName);
            var json = JsonConvert.SerializeObject(name1);
            var name2 = JsonConvert.DeserializeObject<Name>(json);

            (name1 == name2).Should().BeTrue();
        }

        private static class Data
        {
            public const string FirstName = "James";
            public const string LastName = "Kirk";
            public const string MiddleName = "Tiberius";
        }
    }
}