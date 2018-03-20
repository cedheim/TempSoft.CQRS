using System;
using System.Threading.Tasks;

namespace NCG.NGS.CQRS.Queries
{
    public interface IQueryModelRepository
    {
        Task<T> Get<T>(string id);

        Task Save<T>(string id, T model);
    }
}