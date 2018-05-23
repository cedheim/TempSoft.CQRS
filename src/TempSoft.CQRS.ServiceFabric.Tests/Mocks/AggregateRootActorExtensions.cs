using TempSoft.CQRS.Domain;
using TempSoft.CQRS.ServiceFabric.Domain;

namespace TempSoft.CQRS.ServiceFabric.Tests.Mocks
{
    public static class AggregateRootActorExtensions
    {
        public static TRoot GetRoot<TRoot>(this AggregateRootActor actor) where TRoot : IAggregateRoot
        {
            return actor.GetPropertyValue<TRoot>("_root");
        }
    }
}