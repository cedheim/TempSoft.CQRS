using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Queries;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.Queries
{
    [TestFixture]
    public class When_applying_a_series_of_events_to_the_query_builder
    {
        private IQueryModelRepository _repository;
        private IQueryBuilder _builder;
        private AThingQueryModel _model;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _repository = A.Fake<IQueryModelRepository>();

            A.CallTo(() =>
                    _repository.Save(A<string>.Ignored, A<AThingQueryModel>.Ignored, A<CancellationToken>.Ignored))
                .ReturnsLazily(fac =>
                {
                    _model = fac.Arguments.Get<AThingQueryModel>(1);
                    return Task.FromResult(true);
                });
            A.CallTo(() => _repository.Get<AThingQueryModel>(A<string>.Ignored, A<CancellationToken>.Ignored))
                .ReturnsLazily(fac => Task.FromResult(_model));

            _builder = new AThingQueryBuilder(_repository);

            await _builder.Apply(new CreatedAThing(Data.RootId), CancellationToken.None);
            await _builder.Apply(new ChangedAValue(Data.AValue) {AggregateRootId = Data.RootId},
                CancellationToken.None);
            await _builder.Apply(new ChangedBValue(Data.BValue) {AggregateRootId = Data.RootId},
                CancellationToken.None);
        }

        private static class Data
        {
            public const int AValue = 5;
            public const string BValue = "TEST";
            public static readonly Guid RootId = Guid.NewGuid();
        }

        [Test]
        public void Should_have_saved_the_query_model_in_the_repository()
        {
            var modelId = Data.RootId.ToString();
            A.CallTo(
                    () => _repository.Save(modelId, A<AThingQueryModel>.That.IsNotNull(), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Test]
        public void Should_have_updated_the_model()
        {
            _model.Should().NotBeNull();
            _model.A.Should().Be(Data.AValue);
            _model.B.Should().Be(Data.BValue);
        }
    }
}