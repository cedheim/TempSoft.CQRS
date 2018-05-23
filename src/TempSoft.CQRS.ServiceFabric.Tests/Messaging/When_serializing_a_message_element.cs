using System.IO;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Tests.Messaging
{
    [TestFixture]
    public class When_serializing_a_message_element
    {
        private MessageBody _input;
        private MessageBody _output;
        private string _xml;


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _input = new MessageBody("HelloWorld");
            var serializer = new DataContractSerializer(typeof(MessageBody));

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, _input);
                stream.Flush();

                _xml = Encoding.UTF8.GetString(stream.ToArray());

                stream.Seek(0, SeekOrigin.Begin);

                _output = serializer.ReadObject(stream) as MessageBody;
            }
        }

        [Test]
        public void Should_have_resulted_in_the_same_object()
        {
            _output.Should().BeEquivalentTo(_input);
        }
    }
}