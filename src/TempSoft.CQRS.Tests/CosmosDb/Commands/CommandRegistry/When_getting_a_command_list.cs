using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using TempSoft.CQRS.CosmosDb.Commands;
using TempSoft.CQRS.CosmosDb.Events;
using TempSoft.CQRS.CosmosDb.Infrastructure;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.CosmosDb.Commands.CommandRegistry
{
    [TestFixture]
    public class When_getting_a_command_list
    {
        private IDocumentClient _client;
        private CosmosDbCommandRegistry _repository;
        private Database _database;
        private ICosmosDbQueryPager _pager;
        private CommandRegistryWrapper[] _commands;
        private Guid[] _result;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _commands = new [] { new CommandRegistryWrapper(Data.AggregateRootId1, Data.CommandId1), new CommandRegistryWrapper(Data.AggregateRootId2, Data.CommandId2) };

            _client = A.Fake<IDocumentClient>();
            _pager = A.Fake<ICosmosDbQueryPager>();
            _database = new Database(){ Id = Data.DatabaseId };
            A.CallTo(() => _client.CreateDatabaseQuery(A<FeedOptions>.Ignored)).Returns(Enumerable.Empty<Database>().AsQueryable().OrderBy(db => db.Id));
            A.CallTo(() => _client.CreateDatabaseAsync(A<Database>.Ignored, A<RequestOptions>.Ignored))
                .Returns(new ResourceResponse<Database>(_database));
            A.CallTo(() => _client.ReadDocumentCollectionFeedAsync(A<string>.Ignored, A<FeedOptions>.Ignored))
                .Returns(new FeedResponse<DocumentCollection>(Enumerable.Empty<DocumentCollection>()));

            A.CallTo(() => _pager.CreatePagedQuery(A<IQueryable<CommandRegistryWrapper>>.Ignored))
                .ReturnsLazily(foc => new MockDocumentQuery<CommandRegistryWrapper>(foc.GetArgument<IQueryable<CommandRegistryWrapper>>(0)));
            A.CallTo(() => _client.CreateDocumentQuery<CommandRegistryWrapper>(A<Uri>.Ignored, A<FeedOptions>.Ignored))
                .Returns(_commands.AsQueryable().OrderBy(e => e.Id));

            _repository = new CosmosDbCommandRegistry(_client, _pager, Data.DatabaseId, Data.Collectionid);
            _result = (await _repository.Get(Data.AggregateRootId1, CancellationToken.None)).ToArray();
        }

        [Test]
        public void Should_have_called_the_client()
        {
            A.CallTo(() => _client.CreateDocumentQuery<CommandRegistryWrapper>(UriFactory.CreateDocumentCollectionUri(Data.DatabaseId, Data.Collectionid), A<FeedOptions>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_returned_the_correct_event()
        {
            _result.Should().BeEquivalentTo(_commands.Where(cmd => cmd.AggregateRootId == Data.AggregateRootId1).Select(cmd => cmd.Id));
        }


        private static class Data
        {
            public const string DatabaseId = "tempsoft";
            public const string Collectionid = "commands";
            
            public static readonly Guid AggregateRootId1 = Guid.NewGuid();
            public static readonly Guid CommandId1 = Guid.NewGuid();
            public static readonly Guid AggregateRootId2 = Guid.NewGuid();
            public static readonly Guid CommandId2 = Guid.NewGuid();
        }
    }
}