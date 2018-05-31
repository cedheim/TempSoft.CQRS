using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace TempSoft.CQRS.CosmosDb.Tests.Mocks
{
    public class MockDocumentQuery<T> : IDocumentQuery<T>
    {
        private T[] _elements;

        public MockDocumentQuery(IEnumerable<T> elements)
        {
            HasMoreResults = true;
            _elements = elements.ToArray();
        }

        public void Dispose()
        {
            _elements = null;
        }

        public Task<FeedResponse<TResult>> ExecuteNextAsync<TResult>(CancellationToken token = new CancellationToken())
        {
            HasMoreResults = false;
            return Task.FromResult(new FeedResponse<TResult>(_elements.Cast<TResult>()));
        }

        public Task<FeedResponse<dynamic>> ExecuteNextAsync(CancellationToken token = new CancellationToken())
        {
            HasMoreResults = false;
            return Task.FromResult(new FeedResponse<dynamic>(_elements.Cast<dynamic>()));
        }

        public bool HasMoreResults { get; private set; }
    }
}