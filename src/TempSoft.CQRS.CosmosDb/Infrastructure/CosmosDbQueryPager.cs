using System.Linq;
using Microsoft.Azure.Documents.Linq;

namespace TempSoft.CQRS.CosmosDb.Infrastructure
{
    public class CosmosDbQueryPager
        : ICosmosDbQueryPager
    {
        public IDocumentQuery<T> CreatePagedQuery<T>(IQueryable<T> query)
        {
            return query.AsDocumentQuery();
        }
    }
}