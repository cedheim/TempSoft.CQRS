using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using TempSoft.CQRS.Queries;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.Queries
{
    [TestFixture]
    public class When_applying_an_initialization_event_to_the_query_builder
    {
        private IQueryModelRepository _repository;
        private IQueryBuilder _builder;
        private CreatedAThing _event;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _repository = A.Fake<IQueryModelRepository>();
            _builder = new AThingQueryBuilder(_repository);
            _event = new CreatedAThing(Data.RootId);

            await _builder.Apply(_event, CancellationToken.None);
        }

        [Test]
        public void Should_have_saved_the_query_model_in_the_repository()
        {
            var modelId = Data.RootId.ToString();
            A.CallTo(() => _repository.Save(modelId, A<AThingQueryModel>.That.IsNotNull(), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
        }
    }
}