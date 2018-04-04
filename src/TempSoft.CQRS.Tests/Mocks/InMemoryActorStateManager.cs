using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;

namespace TempSoft.CQRS.Tests.Mocks
{
    public class InMemoryActorStateManager : IActorStateManager
    {
        private ConcurrentDictionary<string, object> _saved;
        private ConcurrentDictionary<string, object> _cache;

        public InMemoryActorStateManager()
        {
            _saved = new ConcurrentDictionary<string, object>();
            _cache = new ConcurrentDictionary<string, object>();
        }

        public InMemoryActorStateManager(IEnumerable<KeyValuePair<string, object>> existingValues =
                default(IEnumerable<KeyValuePair<string, object>>))
        {
            _saved = new ConcurrentDictionary<string, object>(existingValues);
            _cache = new ConcurrentDictionary<string, object>(_saved);
        }

        public Task<T> AddOrUpdateStateAsync<T>(string stateName, T addValue, Func<string, T, T> updateValueFactory, CancellationToken cancellationToken = default(CancellationToken))
            => Task.Run(() => (T)_cache.AddOrUpdate(stateName, addValue.Clone(), (s, o) => updateValueFactory(s, (T)o).Clone()) , cancellationToken);

        public Task AddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken)) 
            => Task.Run(() => _cache.TryAdd(stateName, value.Clone()));

        public Task ClearCacheAsync(CancellationToken cancellationToken = default(CancellationToken)) 
            => Task.Run(() => _cache = new ConcurrentDictionary<string, object>(_saved));

        public Task<bool> ContainsStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken)) 
            => Task.Run(() => _cache.ContainsKey(stateName), cancellationToken);

        public Task<T> GetOrAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
         => Task.Run(() => (T)_cache.GetOrAdd(stateName, value.Clone()), cancellationToken);

        public Task<T> GetStateAsync<T>(string stateName, CancellationToken cancellationToken = default(CancellationToken))
            => Task.Run(() => (T)_cache[stateName].Clone(), cancellationToken);

        public Task<IEnumerable<string>> GetStateNamesAsync(CancellationToken cancellationToken = default(CancellationToken))
            => Task.Run(() => _cache.Keys.ToArray().AsEnumerable());

        public Task RemoveStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
            => Task.Run(() => _cache.TryRemove(stateName, out var _), cancellationToken);

        public Task SaveStateAsync(CancellationToken cancellationToken = default(CancellationToken)) 
            => Task.Run(() => _saved = new ConcurrentDictionary<string, object>(_cache));

        public Task SetStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
            => Task.Run(() => _cache[stateName] = value.Clone());

        public Task<bool> TryAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
            => Task.Run(() => _cache.TryAdd(stateName, value.Clone()), cancellationToken);

        public Task<ConditionalValue<T>> TryGetStateAsync<T>(string stateName, CancellationToken cancellationToken = default(CancellationToken))
            => Task.Run(() => new ConditionalValue<T>(_cache.TryGetValue(stateName, out var value), (T)value.Clone()), cancellationToken);

        public Task<bool> TryRemoveStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
            => Task.Run(() => _cache.TryRemove(stateName, out var _), cancellationToken);


        public IDictionary<string, object> AsDictionary()
        {
            return new Dictionary<string, object>(_cache);
        }
    }
}