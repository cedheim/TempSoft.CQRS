using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using TempSoft.CQRS.CosmosDb.Commands;
using TempSoft.CQRS.CosmosDb.Infrastructure;

namespace TempSoft.CQRS.CosmosDb.Tests.Commands.CommandRegistry
{
    [TestFixture]
    public class When_saving_a_command_list
    {
        private IDocumentClient _client;
        private CosmosDbCommandRegistry _repository;
        private Database _database;
        private ICosmosDbQueryPager _pager;
        private Guid[] _commands;
        private Guid[] _result;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _commands = new[] {Data.CommandId1, Data.CommandId2};

            _client = A.Fake<IDocumentClient>();
            _pager = A.Fake<ICosmosDbQueryPager>();
            _database = new Database {Id = Data.DatabaseId};
            A.CallTo(() => _client.CreateDatabaseQuery(A<FeedOptions>.Ignored))
                .Returns(Enumerable.Empty<Database>().AsQueryable().OrderBy(db => db.Id));
            A.CallTo(() => _client.CreateDatabaseAsync(A<Database>.Ignored, A<RequestOptions>.Ignored))
                .Returns(new ResourceResponse<Database>(_database));
            A.CallTo(() => _client.ReadDocumentCollectionFeedAsync(A<string>.Ignored, A<FeedOptions>.Ignored))
                .Returns(new FeedResponse<DocumentCollection>(Enumerable.Empty<DocumentCollection>()));

            _repository = new CosmosDbCommandRegistry(_client, _pager, Data.DatabaseId, Data.Collectionid);
            await _repository.Save(Data.AggregateRootId1, _commands, CancellationToken.None);
        }


        private static class Data
        {
            public const string DatabaseId = "tempsoft";
            public const string Collectionid = "commands";

            public static readonly Guid AggregateRootId1 = Guid.NewGuid();
            public static readonly Guid CommandId1 = Guid.NewGuid();
            public static readonly Guid CommandId2 = Guid.NewGuid();
        }

        [Test]
        public void Should_have_called_the_client()
        {
            A.CallTo(() => _client.UpsertDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(Data.DatabaseId, Data.Collectionid),
                    A<CommandRegistryWrapper>.That.Matches(wrapper =>
                        wrapper.AggregateRootId == Data.AggregateRootId1 && _commands.Contains(wrapper.Id)),
                    A<RequestOptions>.Ignored, A<bool>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Times(_commands.Length));
        }
    }
}