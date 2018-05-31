using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Tests.Messaging
{
    [TestFixture]
    public class When_constructing_a_new_generic_message
    {
        private GenericMessage _message;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _message = new GenericMessage("HelloWorld",
                new[]
                {
                    new KeyValuePair<string, object>("Header1", "AnotherWorld"),
                    new KeyValuePair<string, object>("Header2", "Over Writeable")
                });
        }

        [Test]
        public void Should_be_able_to_add_a_new_header()
        {
            _message.SetHeader("Header3", "AThirdWorld");

            _message.TryGetHeader("Header3", out var header).Should().BeTrue();

            (header as string).Should().BeEquivalentTo("AThirdWorld");
        }

        [Test]
        public void Should_be_able_to_get_the_body_correctly()
        {
            (_message.Body as string).Should().BeEquivalentTo("HelloWorld");
        }

        [Test]
        public void Should_be_able_to_get_the_header()
        {
            _message.TryGetHeader("Header1", out var header).Should().BeTrue();

            (header as string).Should().BeEquivalentTo("AnotherWorld");
        }

        [Test]
        public void Should_be_able_to_overwrite_an_existing_header()
        {
            _message.SetHeader("Header2", "OVERWRITTEN");

            _message.TryGetHeader("Header2", out var value);
            value.ToString().Should().Be("OVERWRITTEN");
        }

        [Test]
        public void Should_contain_the_header()
        {
            _message.HeaderNames.Should().ContainSingle(name => name == "Header1");
            _message.HeaderNames.Should().ContainSingle(name => name == "Header2");
        }

        [Test]
        public void Should_set_the_type_correctly()
        {
            _message.Type.Should().BeSameAs(typeof(string));
        }
    }
}