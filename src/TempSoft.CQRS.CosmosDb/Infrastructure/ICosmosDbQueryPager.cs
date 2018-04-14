using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Azure.Documents.Linq;

namespace TempSoft.CQRS.CosmosDb.Infrastructure
{
    public interface ICosmosDbQueryPager
    {
        IDocumentQuery<T> CreatePagedQuery<T>(IQueryable<T> query);
    }
}
