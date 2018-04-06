using FakeItEasy;
using NUnit.Framework;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Infrastructure;

namespace TempSoft.CQRS.Tests
{
    [SetUpFixture]
    public class GlobalSetUp
    {
        public static ICommandRouter FakeCommandRouter = A.Fake<ICommandRouter>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Services.Register(() => FakeCommandRouter);
        }
    }
}