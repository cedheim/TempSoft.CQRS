using System.IO;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Messaging
{
    [TestFixture]
    public class When_serializing_an_initialize_message
    {
        private InitializeMessage _input;
        private InitializeMessage _output;
        private string _xml;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _input = new InitializeMessage(typeof(AThingAggregateRoot));
            var serializer = new DataContractSerializer(typeof(InitializeMessage));

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, _input);
                stream.Flush();

                _xml = Encoding.UTF8.GetString(stream.ToArray());

                stream.Seek(0, SeekOrigin.Begin);

                _output = serializer.ReadObject(stream) as InitializeMessage;
            }
        }

        [Test]
        public void Should_have_input_equal_to_output()
        {
            _output.Should().NotBeNull();

            _output.Should().BeEquivalentTo(_input);
        }
    }
}