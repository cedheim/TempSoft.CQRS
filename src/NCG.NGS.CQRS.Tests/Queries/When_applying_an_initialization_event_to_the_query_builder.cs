using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using NCG.NGS.CQRS.Queries;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.Queries
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
            A.CallTo(() => _repository.Save<AThingQueryModel>(modelId, A<AThingQueryModel>.That.IsNotNull()))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
        }
    }
}