using ModelCache.Api.EventArgs;
using ModelCache.Api.Models;

namespace ModelCache.Api
{
    public interface IModelService
    {
        public Task Initialize();

        /// <summary>
        ///     Stores a model in the cache and triggers the create effect for that type.
        /// </summary>
        /// <remarks>
        ///     Does not overwrite existing models.
        ///     Anm.d.R.: Exception in dem Fall ratsam.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of the new model.</typeparam>
        /// <param name="model">The new model.</param>
        /// <returns>
        ///     The created model as it is stored in the cache.
        /// </returns>
        public Task<T?> CreateModel<T>(T model) where T : IModel;

        public Task<IEnumerable<T>> CreateModels<T>(IEnumerable<T> models) where T : IModel;

        public Task<T?> ReadModelAsync<T>(Guid id) where T : IModel;

        public T? ReadModel<T>(Guid id) where T : IModel;

        /// <summary>
        ///     Updates or creates a model in the cache and triggers the update effect for that type.
        /// </summary>
        /// <remarks>
        ///     If the model does not already exist, it is created.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of the model.</typeparam>
        /// <param name="model">The changed model.</param>
        /// <returns>
        ///     The updated model as it is stored in the cache.
        /// </returns>
        public Task<T?> UpdateModel<T>(T model) where T : IModel;

        public Task<IEnumerable<T>> UpdateModels<T>(IEnumerable<T> models) where T : IModel;

        public Task DeleteModel<T>(Guid id) where T : IModel;

        public Task DeleteModel<T>(T model) where T : IModel;


        public Task<IQueryable<T>> ReadModelsAsync<T>() where T : IModel;

        public Task<IQueryable<T>> ReadModelsAsync<T>(int skip, int take) where T : IModel;

        public Task<IQueryable<T>> ReadModelsAsync<T>(Dictionary<string, object> options) where T : IModel;

        public Task<IQueryable<T>> ReadModelsAsync<T>(ModelSearchOptionsBase options) where T : IModel;

        public Task<IQueryable<T>> ReadModelsAsync<T>(ModelSearchOptionsBase options, Dictionary<string, object> additionalOptions) where T : IModel;

        public IQueryable<T> ReadModels<T>() where T : IModel;

        public IQueryable<T> ReadModels<T>(int skip, int take) where T : IModel;

        public IQueryable<T> ReadModels<T>(Dictionary<string, object> options) where T : IModel;

        public IQueryable<T> ReadModels<T>(ModelSearchOptionsBase options) where T : IModel;

        public IQueryable<T> ReadModels<T>(ModelSearchOptionsBase options, Dictionary<string, object> additionalOptions) where T : IModel;



        public event EventHandler<ModelChangedEventArgs> ModelChanged;
    }
}
