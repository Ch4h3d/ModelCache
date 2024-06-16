using ModelCache.Api.Models;

namespace ModelCache.Api
{
    public interface IModelSynchronizationService
    {
        public Task Initialize();

        public void AddModelCreateEffect<T>(Func<IServiceProvider, object, Task<object?>> createEffect);

        public void AddModelReadEffect<T>(Func<IServiceProvider, Guid, object[], Task<object?>> readEffect);

        public void AddModelReadManyEffect<T>(Func<IServiceProvider, ModelSearchOptionsBase, Dictionary<string, object>, Task<object[]>> readEffect);

        public void AddModelReadManyFromCacheEffect<T>(Func<IQueryable<object>, ModelSearchOptionsBase, Dictionary<string, object>, IQueryable<object>> readEffect);

        public void AddModelUpdateEffect<T>(Func<IServiceProvider, object, Task<object?>> updateEffect);

        public void AddModelDeleteEffect<T>(Func<IServiceProvider, Guid, Task> deleteEffect);


        public void AddModelCreateEffect(Type modelType, Func<IServiceProvider, object, Task<object?>> createEffect);

        public void AddModelReadEffect(Type modelType,
            Func<IServiceProvider, Guid, object[], Task<object?>> readEffect);

        public void AddModelReadManyEffect(Type modelType,
            Func<IServiceProvider, ModelSearchOptionsBase, Dictionary<string, object>, Task<object[]>> readEffect);

        public void AddModelReadManyFromCacheEffect(Type modelType,
            Func<IQueryable<object>, ModelSearchOptionsBase, Dictionary<string, object>, IQueryable<object>>
                readEffect);

        public void AddModelUpdateEffect(Type modelType, Func<IServiceProvider, object, Task<object?>> updateEffect);

        public void AddModelDeleteEffect(Type modelType, Func<IServiceProvider, Guid, Task> deleteEffect);
        public Task<T?> CreateModel<T>(T model);

        public Task<T?> ReadModel<T>(Guid id);

        public Task<IEnumerable<T>> ReadManyModels<T>(ModelSearchOptionsBase options, Dictionary<string, object> additionalOptions);

        public IQueryable<T> ReadManyModelsFromCache<T>(IQueryable<T> q, ModelSearchOptionsBase options, Dictionary<string, object> additionalOptions);

        public Task<T?> UpdateModel<T>(T model);

        public Task DeleteModel<T>(Guid id);
    }
}
