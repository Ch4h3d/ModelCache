using ModelCache.Api.EventArgs;
using ModelCache.Api.Models;
using System.Collections;

namespace ModelCache.Api
{
    public interface IModelCacheService
    {

        public IQueryable<T> RetrieveModels<T>() where T : IModel;

        public T? RetrieveModel<T>(Guid id) where T : IModel;

        public bool ModelExists<T>(Guid id) where T : IModel;

        /// <summary>
        ///     Stores models in cache.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="model">The model instance.</param>
        /// <param name="overwrite">States, whether an existing model is overwriten.</param>
        /// <param name="updated">States, wheter a changed notification is emitted, if the model already exists.</param>
        public void StoreModel<T>(T model, bool overwrite = true, bool updated = false) where T : IModel;

        public void StoreModels<T>(IEnumerable<T> models, bool overwrite = true, bool updated = false) where T : IModel;

        public void DeleteModel<T>(Guid id) where T : IModel;

        public bool AreCacheElementsValid<T>(IEnumerable<T> models) where T : IModel;

        public bool AreCacheElementsValid<T>(IEnumerable<Guid> ids) where T : IModel;

        public bool IsCacheElementValid<T>(T model) where T : IModel;

        public bool IsCacheElementValid<T>(Guid id) where T : IModel;

        public event EventHandler<ModelChangedEventArgs> ModelChanged;
    }
}
