using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.InMemory.Infrastructure;

namespace TempSoft.CQRS.CosmosDb.Tests.Infrastructure
{
    [TestFixture]
    [Category("Integration")]
    public class When_bootstrapping_cosmosdb
    {
        private FluentBootstrapper _bootstrapper;

        [SetUp]
        public void SetUp()
        {
            _bootstrapper = new FluentBootstrapper();
        }

        [TearDown]
        public void TearDown()
        {
            _bootstrapper.Dispose();
        }

        [Test]
        public void Should_be_able_to_configure_bootstrapper()
        {
            _bootstrapper.UseCosmosDb(new Uri("https://localhost:8081"), "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==")
                .UseCosmosDbCommandRegistry(Data.DatabaseId, Data.CommandsCollectionId)
                .UseCosmosDbEventStore(Data.DatabaseId, Data.EventsCollectionId)
                .UseCosmosDbProjectionModelRepository(Data.DatabaseId, Data.ProjectionsCollectionid)
                .UseInMemoryCommandRouter()
                .UseInMemoryEventBus()
                .Validate();
        }

        private static class Data
        {
#if NETCOREAPP2_0
            public const string DatabaseId = "tempsoft_cqrs_tests_core";
#else
            public const string DatabaseId = "tempsoft_cqrs_tests_net452";
#endif
            public const string EventsCollectionId = "events";
            public const string CommandsCollectionId = "commands";
            public const string ProjectionsCollectionid = "projections";
        }

    }
}
