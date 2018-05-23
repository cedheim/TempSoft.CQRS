using System;
using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using ServiceFabric.Mocks;

namespace TempSoft.CQRS.Tests.Mocks
{
    public static class ServiceFabricFactories
    {
        public static MockActorService<TActor> CreateActorServiceForActorWithCustomStateManager<TActor>(
            Func<ActorService, ActorId, ActorBase> actorFactory = null, IActorStateProvider actorStateProvider = null,
            StatefulServiceContext context = null, ActorServiceSettings settings = null,
            Func<ActorBase, IActorStateProvider, IActorStateManager> stateManagerFactory = null)
            where TActor : Actor
        {
            stateManagerFactory = stateManagerFactory ?? ((actr, stateProvider) => new MockActorStateManager());
            if (actorStateProvider == null)
            {
                actorStateProvider = new MockActorStateProvider();
                actorStateProvider.Initialize(ActorTypeInformation.Get(typeof(TActor)));
            }

            context = context ?? MockStatefulServiceContextFactory.Default;
            var svc = new MockActorService<TActor>(context, ActorTypeInformation.Get(typeof(TActor)), actorFactory,
                stateManagerFactory, actorStateProvider, settings);
            return svc;
        }

        public static IActorStateManager CreateAFakeActorStateManager(
            IDictionary<string, object> existingState = default(IDictionary<string, object>))
        {
            return new InMemoryActorStateManager(existingState);
        }
    }
}