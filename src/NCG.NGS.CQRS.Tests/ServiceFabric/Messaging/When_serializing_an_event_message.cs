using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Messaging
{
    [TestFixture]
    public class When_serializing_an_event_message
    {
        private EventMessage _input;
        private EventMessage _output;
        private string _xml;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _input = new EventMessage(new ChangedAValue(5) { AggregateRootId = Guid.NewGuid(), Version = 5 });
            var serializer = new DataContractSerializer(typeof(EventMessage));

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, _input);
                stream.Flush();

                _xml = Encoding.UTF8.GetString(stream.ToArray());

                stream.Seek(0, SeekOrigin.Begin);

                _output = serializer.ReadObject(stream) as EventMessage;
            }
        }

        [Test]
        public void Should_have_input_equal_to_output()
        {
            var inputEvent = _input.GetEvent<ChangedAValue>();
            var outputEvent = _output.GetEvent<ChangedAValue>();

            inputEvent.Should().NotBeNull();
            outputEvent.Should().NotBeNull();

            outputEvent.Should().BeEquivalentTo(inputEvent);
        }
    }
}