﻿using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.Tests.ServiceFabric.Messaging
{
    [TestFixture]
    public class When_serializing_a_generic_message
    {
        private GenericMessage _input;
        private string _xml;
        private GenericMessage _output;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _input = new GenericMessage("HelloWorld", headers: new[] { new KeyValuePair<string, object>("Header1", "AnotherWorld") });
            var serializer = new DataContractSerializer(typeof(GenericMessage));

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, _input);
                stream.Flush();

                _xml = Encoding.UTF8.GetString(stream.ToArray());

                stream.Seek(0, SeekOrigin.Begin);

                _output = serializer.ReadObject(stream) as GenericMessage;
            }
        }

        [Test]
        public void Should_be_equivalent_to_the_input()
        {
            _output.Should().BeEquivalentTo(_input);
        }
    }
}