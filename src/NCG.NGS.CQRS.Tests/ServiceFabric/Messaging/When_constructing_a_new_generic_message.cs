using System.Collections.Generic;
using FluentAssertions;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Messaging
{
    [TestFixture]
    public class When_constructing_a_new_generic_message
    {
        private GenericMessage _message;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _message = new GenericMessage("HelloWorld", headers: new[] { new KeyValuePair<string, object>("Header1", "AnotherWorld") });

        }

        [Test]
        public void Should_be_able_to_get_the_body_correctly()
        {
            (_message.Body as string).Should().BeEquivalentTo("HelloWorld");
        }

        [Test]
        public void Should_set_the_type_correctly()
        {
            _message.Type.Should().BeSameAs(typeof(string));
        }

        [Test]
        public void Should_contain_the_header()
        {
            _message.HeaderNames.Should().ContainSingle(name => name == "Header1");
        }

        [Test]
        public void Should_be_able_to_get_the_header()
        {
            _message.TryGetHeader("Header1", out object header).Should().BeTrue();

            (header as string).Should().BeEquivalentTo("AnotherWorld");
        }

        [Test]
        public void Should_not_be_able_to_add_an_existing_header()
        {
            _message.Invoking(message => message.AddHeader("Header1", "SHOULD NOT WORK"))
                .Should().Throw<System.ArgumentException>();
        }

        [Test]
        public void Should_be_able_to_add_a_new_header()
        {
            _message.AddHeader("Header2", "AThirdWorld");

            _message.TryGetHeader("Header2", out object header).Should().BeTrue();

            (header as string).Should().BeEquivalentTo("AThirdWorld");
        }
    }
}