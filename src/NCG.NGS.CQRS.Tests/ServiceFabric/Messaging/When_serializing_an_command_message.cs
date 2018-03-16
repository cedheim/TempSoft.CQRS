using System.IO;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NCG.NGS.CQRS.ServiceFabric.Messaging;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Messaging
{
    [TestFixture]
    public class When_serializing_an_command_message
    {
        private CommandMessage _input;
        private CommandMessage _output;
        private string _xml;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _input = new CommandMessage(typeof(AThingAggregateRoot), new DoSomething(5, "TEST"));
            var serializer = new DataContractSerializer(typeof(CommandMessage));

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, _input);
                stream.Flush();

                _xml = Encoding.UTF8.GetString(stream.ToArray());

                stream.Seek(0, SeekOrigin.Begin);

                _output = serializer.ReadObject(stream) as CommandMessage;
            }
        }

        [Test]
        public void Should_have_input_equal_to_output()
        {
            var inputEvent = _input.GetCommand<DoSomething>();
            var outputEvent = _output.GetCommand<DoSomething>();

            inputEvent.Should().NotBeNull();
            outputEvent.Should().NotBeNull();

            outputEvent.Should().BeEquivalentTo(inputEvent);
        }
    }
}