using FakeItEasy;
using NUnit.Framework;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Infrastructure;

namespace TempSoft.CQRS.Tests
{
    [SetUpFixture]
    public class GlobalSetUp
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }
    }
}