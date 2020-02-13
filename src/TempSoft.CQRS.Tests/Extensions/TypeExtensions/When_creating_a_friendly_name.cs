using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TempSoft.CQRS.Extensions;

namespace TempSoft.CQRS.Tests.Extensions.TypeExtensions
{
    [TestFixture]
    public class When_creating_a_friendly_name
    {
        [Test]
        public void Should_be_class_name_and_assembly_without_version()
        {
            var friendlyName = typeof(TempSoft.CQRS.Mocks.AThingAggregateRoot).ToFriendlyName();

            friendlyName.Should().Be("TempSoft.CQRS.Mocks.AThingAggregateRoot, TempSoft.CQRS.Mocks");
        }

        

    }
}
