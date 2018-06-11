using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.ServiceFabric.Actors;
using NUnit.Framework;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.Projectors;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.ServiceFabric.Projectors;

namespace TempSoft.CQRS.ServiceFabric.Tests.Projectors.ProjectorActors
{
    [TestFixture]
    public class When_projecting_using_an_actor : ProjectorActorTestBase
    {
        private AThingProjector _projector;
        private CreatedAThing _event;
        private ProjectorMessage _message;
        private ActorId _actorId;
        private ProjectorActor _actor;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _projector = new AThingProjector(ProjectionModelRepository) { ProjectorId = Data.ProjectorId };
            _event = new CreatedAThing();
            _message = new ProjectorMessage(_event, typeof(AThingProjector));
            _actorId = new ActorId(Data.ProjectorId);

            A.CallTo(() => ProjectorRepository.Get(A<Type>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .Returns(_projector);

            _actor = ActorService.Activate(_actorId);
            await _actor.Project(_message, CancellationToken.None);
        }

        [Test]
        public void Should_have_gotten_the_projector()
        {
            A.CallTo(() => ProjectorRepository.Get(typeof(AThingProjector), Data.ProjectorId, A<CancellationToken>.Ignored))
                .MustHaveHappened();
        }

        [Test]
        public void Should_have_saved_the_projection()
        {
            A.CallTo(() => ProjectionModelRepository.Save(A<IProjection>.That.IsNotNull(), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private static class Data
        {
            public const string ProjectorId = "Projector";
        }
    }
}