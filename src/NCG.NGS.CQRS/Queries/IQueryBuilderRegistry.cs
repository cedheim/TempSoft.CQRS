using System.Threading.Tasks;
using NCG.NGS.CQRS.Events;

namespace NCG.NGS.CQRS.Queries
{
    public interface IQueryBuilderRegistry
    {
        Task Apply(IEvent @event);
    }
}