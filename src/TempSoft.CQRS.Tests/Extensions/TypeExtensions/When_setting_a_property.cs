using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TempSoft.CQRS.Extensions;

namespace TempSoft.CQRS.Tests.Extensions.TypeExtensions
{
    [TestFixture]
    public class When_setting_a_property
    {
        private ATestClass _aTestClass;

        [SetUp]
        public void SetUp()
        {
            _aTestClass = new ATestClass();
        }

        [Test]
        public void Should_be_able_to_set_a_public_property()
        {
            _aTestClass.SetProperty("A", 5);
            _aTestClass.A.Should().Be(5);
        }

        [Test]
        public void Should_be_able_to_set_a_public_property_with_private_setter()
        {
            _aTestClass.SetProperty("B", 5);
            _aTestClass.B.Should().Be(5);
        }

        [Test]
        public void Should_be_able_to_set_a_private_field()
        {
            _aTestClass.SetProperty("_c", 5);
            _aTestClass.C.Should().Be(5);
        }

        [Test]
        public void Should_be_able_to_set_an_inherited_property()
        {
            _aTestClass.SetProperty("_D", 5);
            _aTestClass.D.Should().Be(5);
        }

        [Test]
        public void Should_be_able_to_set_a_private_property()
        {
            _aTestClass.SetProperty("_E", 5);
            _aTestClass.E.Should().Be(5);
        }

        private class ATestClass : ABaseClass
        {
            private int _c;

            public int A { get; set; }

            public int B { get; private set; }

            private int _E { get; set; }

            public int C => _c;

            public int E => _E;

            public int D => _D;
        }


        private class ABaseClass
        {
            protected int _D { get; set; }


        }
    }

}
