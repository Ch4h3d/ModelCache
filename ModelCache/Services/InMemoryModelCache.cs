using ModelCache.Api;
using ModelCache.Api.EventArgs;
using System.Collections;
using System.Collections.Concurrent;

namespace ModelCache.Services
{
    public class InMemoryModelCache : IModelCacheService
    {
        // Convention type.name + Guid
        private readonly ConcurrentDictionary<string, DateTime> _cacheUpdateTimes = new ConcurrentDictionary<string, DateTime>();

        private TimeSpan CacheUpdateInterval { get; } = TimeSpan.FromMinutes(15);

        private string GetCacheKey<T>(T model) where T : IModel
        {
            return GetCacheKey<T>(model.Id);
        }

        private string GetCacheKey<T>(Guid id) where T : IModel
        {
            return typeof(T).Name + id.ToString();
        }

        public bool AreCacheElementsValid<T>(IEnumerable<T> models) where T : IModel
        {
            return models.All(m => IsCacheElementValid<T>(m));
        }

        public bool AreCacheElementsValid<T>(IEnumerable<Guid> ids) where T : IModel
        {
            return ids.All(i => IsCacheElementValid<T>(i));
        }

        public bool IsCacheElementValid<T>(T model) where T : IModel
        {
            return IsCacheElementValid<T>(model.Id);
        }

        public bool IsCacheElementValid<T>(Guid id) where T : IModel
        {
            if (id == Guid.Empty)
            {
                // Guid.Empty is not considered a valid Id, so only no valid cache.
                return false;
            }
            if (!_cacheUpdateTimes.TryGetValue(GetCacheKey<T>(id), out var cacheTime))
            {
                return false;
            }
            return DateTime.UtcNow - cacheTime < CacheUpdateInterval;
        }

        private ConcurrentDictionary<Type, Hashtable> Models { get; } = new();

        public event EventHandler<ModelChangedEventArgs>? ModelChanged;


        public IQueryable<T> RetrieveModels<T>() where T : IModel
        {
            if (Models.TryGetValue(typeof(T), out var typeTable))
            {
                //  Kopie der Modelliste, um "System.InvalidOperationException: Collection was modified" zu verhindern
                // Anm.d.Red. Speichertechnisch nicht optimal, weil der gesamte Cache copiert wird.
                // Eine Lösung, die nur den relevanten Teil zurückgibt, wenn möglich, wäre besser.
                T[] outmodels = new T[typeTable.Values.Count];
                typeTable.Values.CopyTo(outmodels, 0);
                return outmodels.Cast<T>().AsQueryable();
            }

            return Array.Empty<T>().AsQueryable();
        }

        public T? RetrieveModel<T>(Guid id) where T : IModel
        {
            if (Models.TryGetValue(typeof(T), out var typeTable) &&
                typeTable.ContainsKey(id))
            {
                return (T?)typeTable[id];
            }

            return default;
        }

        public bool ModelExists<T>(Guid id) where T : IModel
        {
            return Models.TryGetValue(typeof(T), out var typeTable) && typeTable[id] is T;
        }

        /// <inheritdoc cref="IModelCacheService.StoreModel{T}(T, bool, bool)"/>
        /// <remarks>
        ///     HACK updated flag should not be nessecary to determine if an updated model has changed... equals?
        /// </remarks>
        public void StoreModel<T>(T model, bool overwrite = true, bool updated = false) where T : IModel
        {
            var modelType = typeof(T);
            if (!Models.ContainsKey(modelType))
            {
                Models[modelType] = new Hashtable();
            }
            var typeHashtable = Models[modelType];
            if (overwrite || !typeHashtable.ContainsKey(model.Id))
            {
                Models[modelType][model.Id] = model;
                if (!IsCacheElementValid(model) || updated)
                {
                    ModelChanged?.Invoke(this, new ModelChangedEventArgs(typeof(T)));
                }
                _cacheUpdateTimes[GetCacheKey(model)] = DateTime.UtcNow;
            }
        }

        public void StoreModels<T>(IEnumerable<T> models, bool overwrite = true, bool updated = false) where T : IModel
        {
            var modelType = typeof(T);
            if (!Models.ContainsKey(modelType))
            {
                Models[modelType] = new Hashtable();
            }
            var typeHashtable = Models[modelType];
            bool changes = false;
            foreach (var model in models)
            {
                if (overwrite || !typeHashtable.ContainsKey(model.Id))
                {
                    Models[modelType][model.Id] = model;
                    if (!IsCacheElementValid(model))
                    {
                        // :(
                        changes = true;
                        _cacheUpdateTimes[GetCacheKey(model)] = DateTime.UtcNow;
                    }
                }
            }
            if (changes || updated)
            {
                ModelChanged?.Invoke(this, new ModelChangedEventArgs(modelType));
            }
        }

        public void DeleteModel<T>(Guid id) where T : IModel
        {
            if (ModelExists<T>(id))
            {
                if (Models.TryGetValue(typeof(T), out var models))
                {
                    models.Remove(id);
                }
            }
        }
    }
}
