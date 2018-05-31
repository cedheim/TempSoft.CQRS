using System.Linq;
using Microsoft.Azure.Documents.Linq;

namespace TempSoft.CQRS.CosmosDb.Infrastructure
{
    public interface ICosmosDbQueryPager
    {
        IDocumentQuery<T> CreatePagedQuery<T>(IQueryable<T> query);
    }
}