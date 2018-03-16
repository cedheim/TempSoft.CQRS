using NCG.NGS.CQRS.Domain;
using NCG.NGS.CQRS.ServiceFabric.Domain;

namespace NCG.NGS.CQRS.Tests.Mocks
{
    public static class AggregateRootActorExtensions
    {
        public static TRoot GetRoot<TRoot>(this AggregateRootActor actor) where TRoot : IAggregateRoot
        {
            return actor.GetPropertyValue<TRoot>("_root");
        }
    }
}