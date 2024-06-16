using ModelCache.Api;
using ModelCache.Api.Models;

namespace ModelCache.Services
{
    public class ModelSynchronizationService : IModelSynchronizationService
    {
        private Dictionary<Type, Func<IServiceProvider, object, Task<object?>>> CreateEffects { get; } = new();
        private Dictionary<Type, Func<IServiceProvider, Guid, object[], Task<object?>>> ReadEffects { get; } = new();
        private Dictionary<Type, Func<IServiceProvider, ModelSearchOptionsBase, Dictionary<string, object>, Task<object[]>>> ReadManyEffects { get; } = new();
        private Dictionary<Type, Func<IQueryable<object>, ModelSearchOptionsBase, Dictionary<string, object>, IQueryable<object>>> ReadManyFromCacheEffects { get; } = new();
        private Dictionary<Type, Func<IServiceProvider, object, Task<object?>>> UpdateEffects { get; } = new();
        private Dictionary<Type, Func<IServiceProvider, Guid, Task>> DeleteEffects { get; } = new();

        private IServiceProvider ServiceProvider { get; }

        public ModelSynchronizationService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void AddModelCreateEffect<T>(Func<IServiceProvider, object, Task<object?>> createEffect)
        {
            CreateEffects.Add(typeof(T), createEffect);
        }

        public void AddModelReadEffect<T>(Func<IServiceProvider, Guid, object[], Task<object?>> readEffect)
        {
            ReadEffects.Add(typeof(T), readEffect);
        }

        public void AddModelReadManyEffect<T>(Func<IServiceProvider, ModelSearchOptionsBase, Dictionary<string, object>, Task<object[]>> readEffect)
        {
            ReadManyEffects.Add(typeof(T), readEffect);
        }

        public void AddModelReadManyFromCacheEffect<T>(Func<IQueryable<object>, ModelSearchOptionsBase, Dictionary<string, object>, IQueryable<object>> readEffect)
        {
            ReadManyFromCacheEffects.Add(typeof(T), readEffect);
        }

        public void AddModelUpdateEffect<T>(Func<IServiceProvider, object, Task<object?>> updateEffect)
        {
            UpdateEffects.Add(typeof(T), updateEffect);
        }

        public void AddModelDeleteEffect<T>(Func<IServiceProvider, Guid, Task> deleteEffect)
        {
            DeleteEffects.Add(typeof(T), deleteEffect);
        }

        public void AddModelCreateEffect(Type modelType, Func<IServiceProvider, object, Task<object?>> createEffect)
        {
            CreateEffects.Add(modelType, createEffect);
        }

        public void AddModelReadEffect(Type modelType, Func<IServiceProvider, Guid, object[], Task<object?>> readEffect)
        {
            ReadEffects.Add(modelType, readEffect);
        }

        public void AddModelReadManyEffect(Type modelType, Func<IServiceProvider, ModelSearchOptionsBase, Dictionary<string, object>, Task<object[]>> readEffect)
        {
            ReadManyEffects.Add(modelType, readEffect);
        }

        public void AddModelReadManyFromCacheEffect(Type modelType, Func<IQueryable<object>, ModelSearchOptionsBase, Dictionary<string, object>, IQueryable<object>> readEffect)
        {
            ReadManyFromCacheEffects.Add(modelType, readEffect);
        }

        public void AddModelUpdateEffect(Type modelType, Func<IServiceProvider, object, Task<object?>> updateEffect)
        {
            UpdateEffects.Add(modelType, updateEffect);
        }

        public void AddModelDeleteEffect(Type modelType, Func<IServiceProvider, Guid, Task> deleteEffect)
        {
            DeleteEffects.Add(modelType, deleteEffect);
        }
        
        public async Task<T?> CreateModel<T>(T model)
        {
            if (model != null &&
                CreateEffects.TryGetValue(typeof(T), out var effect))
            {
                return (T?)await effect.Invoke(ServiceProvider, model);
            }
            else
            {
                throw new NotImplementedException($"No {nameof(CreateEffects)} effect for type {typeof(T).Name} registered.");
            }
            return default;
        }

        public async Task<T?> ReadModel<T>(Guid id)
        {
            if (ReadEffects.TryGetValue(typeof(T), out var effect))
            {
                return (T?)await effect.Invoke(ServiceProvider, id, Array.Empty<object>());
            }
            else
            {
                throw new NotImplementedException($"No {nameof(ReadEffects)} effect for type {typeof(T).Name} registered.");
            }
            return default;
        }

        public async Task<IEnumerable<T>> ReadManyModels<T>(ModelSearchOptionsBase options, Dictionary<string, object> additionalOptions)
        {
            if (ReadManyEffects.TryGetValue(typeof(T), out var effect))
            {
                var r = await effect.Invoke(ServiceProvider, options, additionalOptions);
                return r.Cast<T>().ToArray();
            }
            else
            {
                throw new NotImplementedException($"No {nameof(ReadManyEffects)} effect for type {typeof(T).Name} registered.");
            }
            return Array.Empty<T>();
        }

        public IQueryable<T> ReadManyModelsFromCache<T>(IQueryable<T> q, ModelSearchOptionsBase options, Dictionary<string, object> additionalOptions)
        {
            if (ReadManyFromCacheEffects.TryGetValue(typeof(T), out var effect))
            {
                var r = effect.Invoke(q.Cast<object>(), options, additionalOptions);
                return r.Cast<T>();
            }
            else
            {
                throw new NotImplementedException($"No {nameof(ReadManyEffects)} effect for type {typeof(T).Name} registered.");
            }
            return Enumerable.Empty<T>().AsQueryable();
        }

        public async Task<T?> UpdateModel<T>(T model)
        {
            if (model != null &&
                UpdateEffects.TryGetValue(typeof(T), out var effect))
            {
                return (T?)await effect.Invoke(ServiceProvider, model);
            }
            else
            {
                throw new NotImplementedException($"No {nameof(UpdateModel)} effect for type {typeof(T).Name} registered.");
            }
            return default;
        }

        public async Task DeleteModel<T>(Guid id)
        {
            if (DeleteEffects.TryGetValue(typeof(T), out var effect))
            {
                await effect.Invoke(ServiceProvider, id);
            }
            else
            {
                throw new NotImplementedException($"No {nameof(DeleteModel)} effect for type {typeof(T).Name} registered.");
            }
        }


        public async Task Initialize()
        {

        }
    }
}
