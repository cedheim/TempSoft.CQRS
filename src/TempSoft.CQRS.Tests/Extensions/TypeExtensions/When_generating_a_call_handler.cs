using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Extensions;

namespace TempSoft.CQRS.Tests.Extensions.TypeExtensions
{
    [TestFixture]
    public class When_generating_a_call_handler
    {

        private string _returnValue;
        private ATestClass _object;
        private ATestArgument _argument;

        [SetUp]
        public void SetUp()
        {
            _returnValue = Guid.NewGuid().ToString();
            _object = new ATestClass(_returnValue);
            _argument = new ATestArgument
            {
                A = 5,
                B = Guid.NewGuid().ToString()
            };
        }

        [Test]
        public async Task Should_be_able_to_generate_an_async_call()
        {
            var handler = ATestClass.AnAsyncMethodInfo.GenerateAsyncHandler<ATestClass, ATestArgument>();
            await handler(_object, _argument, CancellationToken.None);

            _object.A.Should().Be(_argument.A);
            _object.B.Should().Be(_argument.B);
        }

        [Test]
        public async Task Should_be_able_to_generate_an_async_call_from_a_sync_method()
        {
            var handler = ATestClass.ASyncMethodInfo.GenerateAsyncHandler<ATestClass, ATestArgument>();
            await handler(_object, _argument, CancellationToken.None);

            _object.A.Should().Be(_argument.A);
            _object.B.Should().Be(_argument.B);
        }

        [Test]
        public async Task Should_be_able_to_generate_an_async_call_with_return_value()
        {
            var handler = ATestClass.AnAsyncMethodWithReturnValueInfo.GenerateAsyncHandlerWithReturnType<ATestClass, ATestArgument, string>();
            var result = await handler(_object, _argument, CancellationToken.None);

            _object.A.Should().Be(_argument.A);
            _object.B.Should().Be(_argument.B);
            result.Should().Be(_returnValue);
        }

        [Test]
        public async Task Should_be_able_to_generate_an_async_call_with_return_value_from_a_sync_method()
        {
            var handler = ATestClass.ASyncMethodWithReturnValueInfo.GenerateAsyncHandlerWithReturnType<ATestClass, ATestArgument, string>();
            var result = await handler(_object, _argument, CancellationToken.None);

            _object.A.Should().Be(_argument.A);
            _object.B.Should().Be(_argument.B);
            result.Should().Be(_returnValue);
        }

        [Test]
        public void Should_be_able_to_generate_a_sync_handler()
        {
            var handler = ATestClass.ASyncMethodInfo.GenerateHandler<ATestClass, ATestArgument>();
            handler(_object, _argument);
        }

        [Test]
        public async Task Should_be_able_to_treat_an_async_method_with_return_value_as_void()
        {
            var handler = ATestClass.AnAsyncMethodWithReturnValueInfo.GenerateAsyncHandler<ATestClass, ATestArgument>();
            await handler(_object, _argument, CancellationToken.None);

            _object.A.Should().Be(_argument.A);
            _object.B.Should().Be(_argument.B);
        }

        [Test]
        public void Should_not_be_able_to_create_a_sync_handler_for_a_handler_that_does_not_inherit_properly()
        {
            Action method = () => ATestClass.AnAsyncMethodWithReturnValueInfo.GenerateHandler<ATestClass, ATestArgument>(typeof(string));
            method.Should().Throw<Exception>();
        }

        [Test]
        public void Should_not_be_able_to_create_an_async_handler_for_a_handler_that_does_not_inherit_properly()
        {
            Action method = () => ATestClass.AnAsyncMethodWithReturnValueInfo.GenerateAsyncHandler<ATestClass, ATestArgument>(typeof(string));
            method.Should().Throw<Exception>();
        }

        [Test]
        public void Should_not_be_able_to_create_an_async_handler_with_a_return_value_for_a_handler_that_does_not_inherit_properly()
        {
            Action method = () => ATestClass.AnAsyncMethodWithReturnValueInfo.GenerateAsyncHandlerWithReturnType<ATestClass, ATestArgument, string>(typeof(string));
            method.Should().Throw<Exception>();
        }

        [Test]
        public async Task Should_be_able_to_generate_an_async_call_with_argument_inheritance()
        {
            var handler = ATestClass.AnAsyncMethodInfo.GenerateAsyncHandler<ATestClass, ITestArgument>(typeof(ATestArgument));
            await handler(_object, _argument, CancellationToken.None);

            _object.A.Should().Be(_argument.A);
            _object.B.Should().Be(_argument.B);
        }

        [Test]
        public async Task Should_not_be_able_to_generate_an_async_call_with_inverted_argument_inheritance()
        {
            Action method = () => ATestClass.AnAsyncMethodInfo.GenerateAsyncHandler<ATestClass, ATestArgument>(typeof(ITestArgument));
            method.Should().Throw<Exception>();
        }

        private class ATestClass
        {
            public static MethodInfo AnAsyncMethodInfo = typeof(ATestClass).GetMethod(nameof(AnAsyncMethod));
            public static MethodInfo ASyncMethodInfo = typeof(ATestClass).GetMethod(nameof(ASyncMethod));
            public static MethodInfo AnAsyncMethodWithReturnValueInfo = typeof(ATestClass).GetMethod(nameof(AnAsyncMethodWithReturnValue));
            public static MethodInfo ASyncMethodWithReturnValueInfo = typeof(ATestClass).GetMethod(nameof(ASyncMethodWithReturnValue));

            private readonly string _returnValue;

            public ATestClass()
            {
                _returnValue = Guid.NewGuid().ToString();
            }

            public ATestClass(string returnValue)
            {
                _returnValue = returnValue;
            }

            public int A { get; set; }

            public string B { get; set; }

            public Task AnAsyncMethod(int a, string b)
            {
                return Task.FromResult(ASyncMethodWithReturnValue(a, b));
            }

            public Task<string> AnAsyncMethodWithReturnValue(int a, string b)
            {
                return Task.FromResult(ASyncMethodWithReturnValue(a, b));
            }

            public string ASyncMethodWithReturnValue(int a, string b)
            {
                A = a;
                B = b;
                return _returnValue;
            }

            public void ASyncMethod(int a, string b)
            {
                ASyncMethodWithReturnValue(a, b);
            }
        }

        private interface ITestArgument
        {

        }

        private class ATestArgument : ITestArgument
        {
            public int A { get; set; }

            public string B { get; set; }

        }
    }
}
