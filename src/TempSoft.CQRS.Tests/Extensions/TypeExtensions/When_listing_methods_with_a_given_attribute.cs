using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TempSoft.CQRS.Extensions;

namespace TempSoft.CQRS.Tests.Extensions.TypeExtensions
{
    [TestFixture]
    public class When_listing_methods_with_a_given_attribute
    {
        [Test]
        public void Should_list_both_public_and_private_methods()
        {
            var methods = typeof(TestClass).GetMethodsForAttribute<TestMethodAttribute>().ToArray();

            methods.Should().ContainSingle(m => m.Name == "APublicMethod");
            methods.Should().ContainSingle(m => m.Name == "APrivateMethod");
            methods.Should().HaveCount(2);
        }

        [AttributeUsage(AttributeTargets.Method)]
        private class TestMethodAttribute : Attribute
        {
        }

        private class TestClass
        {
            [TestMethod]
            public void APublicMethod()
            {

            }

            [TestMethod]
            private void APrivateMethod()
            {

            }
        }
    }
}
