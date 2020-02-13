using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TempSoft.CQRS.Extensions;

namespace TempSoft.CQRS.Tests.Extensions.TypeExtensions
{
    [TestFixture]
    public class When_getting_a_property
    {
        private ATestClass _aTestClass;

        [SetUp]
        public void SetUp()
        {
            _aTestClass = new ATestClass();
        }

        [Test]
        public void Should_be_able_to_get_a_public_property()
        {
            _aTestClass.SetProperty("A", 5);
            _aTestClass.GetProperty("A").Should().Be(5);
        }

        [Test]
        public void Should_be_able_to_get_a_public_property_with_private_getter()
        {
            _aTestClass.SetProperty("B", 5);
            _aTestClass.GetProperty("B").Should().Be(5);
        }

        [Test]
        public void Should_be_able_to_get_a_private_field()
        {
            _aTestClass.SetProperty("_c", 5);
            _aTestClass.GetProperty("_c").Should().Be(5);
        }

        [Test]
        public void Should_be_able_to_get_an_inherited_property()
        {
            _aTestClass.SetProperty("D", 5);
            _aTestClass.GetProperty("D").Should().Be(5);
        }

        [Test]
        public void Should_be_able_to_get_a_private_property()
        {
            _aTestClass.SetProperty("E", 5);
            _aTestClass.GetProperty("E").Should().Be(5);
        }

        private class ATestClass : ABaseClass
        {
            private int _c;

            public int A { get; set; }

            public int B { private get; set; }

            private int E { get; set; }
        }


        private class ABaseClass
        {
            protected int D { get; set; }


        }
    }

}
