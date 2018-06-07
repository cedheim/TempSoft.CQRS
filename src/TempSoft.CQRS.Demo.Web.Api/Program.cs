using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Demo.Api;
using TempSoft.CQRS.Demo.Infrastructure;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.InMemory.Infrastructure;

namespace TempSoft.CQRS.Demo.Web.Api
{
    public class Program
    {
        private class CosmosDbDatabase
        {
            public string DatabaseId { get; set; }
            public string CollectionId { get; set; }
        }

        private class CosmosDbConfiguration
        {
            public Uri EndpointUri { get; set; }
            public string PrimaryKey { get; set; }
            public CosmosDbDatabase CommandRegistry { get; set; }
            public CosmosDbDatabase EventStore { get; set; }
            public CosmosDbDatabase ProjectionModelRepository { get; set; }
        }

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();

            var dbConfiguration = new CosmosDbConfiguration
            {
                CommandRegistry = new CosmosDbDatabase(),
                EventStore = new CosmosDbDatabase(),
                ProjectionModelRepository = new CosmosDbDatabase()
            };

            configuration.GetSection("CosmosDbConfiguration").Bind(dbConfiguration);

            var bootstrapper = BootstrapperGenerator.Generate()
                .UseCosmosDb(dbConfiguration.EndpointUri, dbConfiguration.PrimaryKey)
                .UseCosmosDbCommandRegistry(dbConfiguration.CommandRegistry.DatabaseId, dbConfiguration.CommandRegistry.CollectionId)
                .UseCosmosDbEventStore(dbConfiguration.EventStore.DatabaseId, dbConfiguration.EventStore.CollectionId)
                .UseCosmosDbProjectionModelRepository(dbConfiguration.ProjectionModelRepository.DatabaseId, dbConfiguration.ProjectionModelRepository.CollectionId)
                .UseInMemoryCommandRouter()
                .UseInMemoryEventBus()
                .UseInMemoryProjectionQueryRouter()
                .Validate();

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(svc => svc.Add(new ServiceDescriptor(typeof(FluentBootstrapper), bootstrapper)))
                .UseStartup<Startup>()
                .Build();
        }
            
    }
}
