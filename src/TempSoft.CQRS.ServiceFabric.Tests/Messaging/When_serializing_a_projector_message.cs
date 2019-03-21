using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Tests.Messaging
{
    [TestFixture]
    public class When_serializing_a_projector_message
    {
        private ProjectorMessage _input;
        private ProjectorMessage _output;
        private string _xml;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _input = new ProjectorMessage(new ChangedAValue(5) {AggregateRootId = Guid.NewGuid().ToString(), Version = 5}, typeof(AThingProjector));
            var serializer = new DataContractSerializer(typeof(ProjectorMessage));

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, _input);
                stream.Flush();

                _xml = Encoding.UTF8.GetString(stream.ToArray());

                stream.Seek(0, SeekOrigin.Begin);

                _output = serializer.ReadObject(stream) as ProjectorMessage;
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

            _output.ProjectorType.Should().Be(_input.ProjectorType);
        }
    }
}