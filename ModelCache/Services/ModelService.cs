using ModelCache.Api;
using ModelCache.Api.EventArgs;
using ModelCache.Api.Models;

namespace ModelCache.Services
{
    public class ModelService : IModelService, IDisposable
    {
        

        public event EventHandler<ModelChangedEventArgs>? ModelChanged;

        private IModelCacheService ModelCacheService { get; }

        private IModelSynchronizationService ModelSynchronizationService { get; }

        public ModelService(IModelSynchronizationService modelSynchronizationService, IModelCacheService modelCacheService)
        {
            ModelSynchronizationService = modelSynchronizationService;
            ModelCacheService = modelCacheService;
            modelCacheService.ModelChanged += ModelCacheService_ModelChanged;
        }

        private void ModelCacheService_ModelChanged(object? sender, ModelChangedEventArgs e)
        {
            ModelChanged?.Invoke(this, e);
        }

        /// <inheritdoc cref="IModelService.CreateModel{T}(T)">
        public async Task<T?> CreateModel<T>(T model) where T : IModel
        {
            var newModel = await ModelSynchronizationService.CreateModel(model);
            if (newModel != null)
            {
                ModelCacheService.StoreModel(newModel);
                return newModel;
            }

            return default;
        }

        public async Task<IEnumerable<T>> CreateModels<T>(IEnumerable<T> models) where T : IModel
        {
            var newModels = new List<T>();

            foreach (var model in models)
            {
                if (await ModelSynchronizationService.CreateModel(model) is { } newModel)
                {
                    newModels.Add(newModel);
                }
            }

            ModelCacheService.StoreModels(newModels, false);
            return newModels;
        }

        public async Task<T?> ReadModelAsync<T>(Guid id) where T : IModel
        {
            if (!ModelCacheService.ModelExists<T>(id) || !ModelCacheService.IsCacheElementValid<T>(id))
            {
                var newModel = await ModelSynchronizationService.ReadModel<T>(id);
                if (newModel != null)
                {
                    ModelCacheService.StoreModel(newModel);
                    return newModel;
                }
                return default;
            }

            return ModelCacheService.RetrieveModel<T>(id);
        }


        public T? ReadModel<T>(Guid id) where T : IModel
        {
            var model = ModelCacheService.RetrieveModel<T>(id);

            Task.Run(() => ReadModelAsync<T>(id));

            return model;
        }

        /// <inheritdoc cref="IModelService.UpdateModel{T}(T)">
        public async Task<T?> UpdateModel<T>(T model) where T : IModel
        {
            var newModel = await ModelSynchronizationService.UpdateModel(model);
            if (newModel != null)
            {
                ModelCacheService.StoreModel(newModel, updated: true);
                return newModel;
            }
            return default;
        }

        public async Task<IEnumerable<T>> UpdateModels<T>(IEnumerable<T> models) where T : IModel
        {

            var newModels = new List<T>();

            foreach (var model in models)
            {
                if (await ModelSynchronizationService.UpdateModel(model) is { } newModel)
                {
                    newModels.Add(newModel);
                }
            }

            ModelCacheService.StoreModels(newModels, updated: true);
            return newModels;
        }

        public async Task DeleteModel<T>(Guid id) where T : IModel
        {
            // Anm. d. Red. Ermöglicht nur das löschen vorhandener Model,
            // es kann sein, dass das Model noch nicht gecacht wurde, trotzdem kann es gelöscht werden (relevant?)
            if (ModelCacheService.ModelExists<T>(id))
            {
                await ModelSynchronizationService.DeleteModel<T>(id);
                ModelCacheService.DeleteModel<T>(id);
            }

            ModelChanged?.Invoke(this, new ModelChangedEventArgs(typeof(T)));
        }

        public async Task DeleteModel<T>(T model) where T : IModel
        {
            await DeleteModel<T>(model.Id);
        }

        public Task Initialize()
        {
            return Task.CompletedTask;
        }


        public IQueryable<T> ReadModels<T>() where T : IModel
        {
            return ReadModelsInternal<T>(new ModelSearchOptionsBase(), null);
        }

        public IQueryable<T> ReadModels<T>(int skip, int take) where T : IModel
        {
            return ReadModelsInternal<T>(new ModelSearchOptionsBase(skip, take), null);
        }

        public IQueryable<T> ReadModels<T>(Dictionary<string, object> options) where T : IModel
        {
            var optionsBase = new ModelSearchOptionsBase();
            if (options.ContainsKey("skip") && options["skip"] is int i)
            {
                optionsBase.Skip = i;
            }
            if (options.ContainsKey("take") && options["take"] is { } t)
            {
                optionsBase.Take = t as int?;
            }

            return ReadModelsInternal<T>(optionsBase, options);
        }

        public IQueryable<T> ReadModels<T>(ModelSearchOptionsBase options) where T : IModel
        {
            return ReadModelsInternal<T>(options);
        }

        public IQueryable<T> ReadModels<T>(ModelSearchOptionsBase options, Dictionary<string, object> additionalOptions) where T : IModel
        {
            return ReadModelsInternal<T>(options, additionalOptions);
        }


        public async Task<IQueryable<T>> ReadModelsAsync<T>() where T : IModel
        {
            return await ReadModelsInternalAsync<T>(new ModelSearchOptionsBase(), null);
        }

        public async Task<IQueryable<T>> ReadModelsAsync<T>(int skip, int take) where T : IModel
        {
            return await ReadModelsInternalAsync<T>(new ModelSearchOptionsBase(skip, take), null);
        }

        public async Task<IQueryable<T>> ReadModelsAsync<T>(Dictionary<string, object> options) where T : IModel
        {
            var optionsBase = new ModelSearchOptionsBase();
            if (options.ContainsKey("skip") && options["skip"] is int i)
            {
                optionsBase.Skip = i;
            }
            if (options.ContainsKey("take") && options["take"] is { } t)
            {
                optionsBase.Take = t as int?;
            }

            return await ReadModelsInternalAsync<T>(optionsBase, options);
        }

        public async Task<IQueryable<T>> ReadModelsAsync<T>(ModelSearchOptionsBase options) where T : IModel
        {
            return await ReadModelsInternalAsync<T>(options);
        }

        public async Task<IQueryable<T>> ReadModelsAsync<T>(ModelSearchOptionsBase options, Dictionary<string, object> additionalOptions) where T : IModel
        {
            return await ReadModelsInternalAsync<T>(options, additionalOptions);
        }

        private IQueryable<T> ReadModelsInternal<T>(ModelSearchOptionsBase options, Dictionary<string, object>? additionalOptions = null) where T : IModel
        {
            // Faden: Instant Cache rückgabe, trigger load, event wenn changes nach update.
            IQueryable<T> models = Enumerable.Empty<T>().AsQueryable();
            models = ReadModelsFromCache<T>(options, additionalOptions);
            
            if (options.Take == null || options.Take == 0 || models.Count() < options.Take || !ModelCacheService.AreCacheElementsValid<T>(models))
            {
                Task.Run(async () => await ReadModelsFromDownstream<T>(options, additionalOptions));
            }
            return models;
        }

        private async Task<IQueryable<T>> ReadModelsInternalAsync<T>(ModelSearchOptionsBase options, Dictionary<string, object>? additionalOptions = null) where T : IModel
        {
            IQueryable<T> models = Enumerable.Empty<T>().AsQueryable();
            if (options.Take != null && options.Take != 0)
            {
                models = ReadModelsFromCache<T>(options, additionalOptions);
            }
            if (options.Take == null || options.Take == 0 || models.Count() < options.Take || !ModelCacheService.AreCacheElementsValid<T>(models))
            {
                await ReadModelsFromDownstream<T>(options, additionalOptions);
            }
            models = ReadModelsFromCache<T>(options, additionalOptions);
            return models;
        }

        private IQueryable<T> ReadModelsFromCache<T>(ModelSearchOptionsBase options, Dictionary<string, object>? additionalOptions = null) where T : IModel
        {
            additionalOptions ??= new Dictionary<string, object>();
            var models = ModelCacheService.RetrieveModels<T>();
            models = ModelSynchronizationService.ReadManyModelsFromCache<T>(models, options, additionalOptions);
            var q = models.Skip(options.Skip);
            if (options.Take != null)
            {
                return q.Take(options.Take.Value);
            }
            else
            {
                return q;
            }
        }

        private async Task<IQueryable<T>> ReadModelsFromDownstream<T>(ModelSearchOptionsBase options, Dictionary<string, object>? additionalOptions = null) where T : IModel
        {
            additionalOptions ??= [];

            // Faden: prüfmechanismus, wann models aus dem Cache entfernt werden können..
            var models = await ModelSynchronizationService.ReadManyModels<T>(options, additionalOptions);
            ModelCacheService.StoreModels<T>(models);
            return models.AsQueryable();

        }

        public void Dispose()
        {
            ModelCacheService.ModelChanged -= ModelCacheService_ModelChanged;
        }
    }
}
