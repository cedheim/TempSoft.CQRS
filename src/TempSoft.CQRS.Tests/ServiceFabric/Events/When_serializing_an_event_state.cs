using System.IO;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.ServiceFabric.Events;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Events
{
    [TestFixture]
    public class When_serializing_an_event_state
    {
        private EventState _input;
        private EventState _output;
        private string _xml;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _input = new EventState(new ChangedAValue(5));
            var serializer = new DataContractSerializer(typeof(EventState));

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, _input);
                stream.Flush();

                _xml = Encoding.UTF8.GetString(stream.ToArray());

                stream.Seek(0, SeekOrigin.Begin);

                _output = serializer.ReadObject(stream) as EventState;
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