using ModelCache.Api;
using ModelCache.Services;
using ModelCache.Test.TestModels;
using Moq;

namespace ModelCache.Test;

public class ModelServiceTest
{
    private Mock<IModelSynchronizationService> _modelSynchronizationServiceMock;
    private Mock<IModelCacheService> _modelCacheService;

    private const int NumModels = 5;

    private IModelService _testObject;
    private readonly Dictionary<Guid, TestModel> _testModels = new();

    #region SimpleTest

    [Fact]
    public async Task TestReadModel()
    {
        await Setup();
        var cachedModelId = Guid.NewGuid();
        const string testString = "TestOne";
        var cachedModel = new TestModel(cachedModelId, testString);


        _modelSynchronizationServiceMock
            .Setup(s => s.ReadModel<TestModel>(
                cachedModelId))
            .Returns(Task.FromResult<TestModel?>(cachedModel));

        // Model exists in Cache, but is not valid (return from cache, load new model
        _modelCacheService
            .Setup(s => s.ModelExists<TestModel>(cachedModelId))
            .Returns(true);
        _modelCacheService
            .Setup(s => s.IsCacheElementValid<TestModel>(cachedModelId))
            .Returns(false);
        _modelCacheService
            .Setup(s => s.RetrieveModel<TestModel>(
                cachedModelId))
            .Returns(cachedModel);


        var actualModel =  _testObject.ReadModel<TestModel>(cachedModelId);

        Assert.Equal(cachedModel, actualModel);
        
        _modelCacheService.Verify(s => s.RetrieveModel<TestModel>(
            It.IsAny<Guid>()), Times.Once);

        // Synchronization Service read model is not awaited
        // HACK: continue, when read model on synchronization Service mock has been called
        await Task.Delay(200);
        _modelSynchronizationServiceMock.Verify(s => s.ReadModel<TestModel>(
            It.IsAny<Guid>()), Times.Once);
        
        actualModel =  await _testObject.ReadModelAsync<TestModel>(cachedModelId);

        Assert.Equal(cachedModel, actualModel);
        
        _modelSynchronizationServiceMock.Verify(s => s.ReadModel<TestModel>(
            It.IsAny<Guid>()), Times.Exactly(2));
        _modelCacheService.Verify(s => s.RetrieveModel<TestModel>(
            It.IsAny<Guid>()), Times.Exactly(1));
    }
    
    [Fact]
    public async Task TestCreateModel()
    {
        await Setup();
        var modelId = Guid.NewGuid();
        var model = new TestModel(modelId);

        TestModel? lastModelSet = null;

        _modelSynchronizationServiceMock
            .Setup(s => s.CreateModel(
                It.IsAny<TestModel>()))
            .Returns<TestModel>(Task.FromResult<TestModel?>);

        _modelCacheService
            .Setup(s => s.StoreModel(
                It.IsAny<TestModel>(),
                It.IsAny<bool>(),
                It.IsAny<bool>())).Callback<TestModel, bool, bool>((m, _, _) => lastModelSet = m);

        Assert.Null(lastModelSet);

        await _testObject.CreateModel(model);

        Assert.Equal(model, lastModelSet);
    }

    [Fact]
    public async Task TestUpdateModel()
    {
        await Setup();
        var modelId = Guid.NewGuid();
        var model = new TestModel(modelId);

        TestModel? lastModelSet = null;

        _modelSynchronizationServiceMock
            .Setup(s => s.UpdateModel(
                It.IsAny<TestModel>()))
            .Returns<TestModel>(Task.FromResult<TestModel?>);

        _modelCacheService
            .Setup(s => s.ModelExists<TestModel>(
                modelId))
            .Returns(true);
        
        _modelCacheService
            .Setup(s => s.StoreModel(
                It.IsAny<TestModel>(),
                It.IsAny<bool>(),
                It.IsAny<bool>())).Callback<TestModel, bool, bool>((m, _, _) => lastModelSet = m);

        Assert.Null(lastModelSet);

        await _testObject.UpdateModel(model);

        Assert.Equal(model, lastModelSet);
    }
    
    
    [Fact]
    public async Task TestDeleteModel()
    {
        await Setup();
        var modelId = Guid.NewGuid();
        var model = new TestModel(modelId);


        _modelSynchronizationServiceMock
            .Setup(s => s.DeleteModel<TestModel>(
                It.IsAny<Guid>()));

        _modelCacheService
            .Setup(s => s.ModelExists<TestModel>(
                modelId))
            .Returns(true);
        
        _modelCacheService
            .Setup(s => s.DeleteModel<TestModel>(
                It.IsAny<Guid>()));
        
        await _testObject.DeleteModel(model);

        _modelSynchronizationServiceMock.Verify(s => s.DeleteModel<TestModel>(
            It.IsAny<Guid>()), Times.Once);
        _modelCacheService.Verify(s => s.DeleteModel<TestModel>(
            It.IsAny<Guid>()), Times.Once);
    }
    #endregion

    #region Setup

    private async Task Setup()
    {
        _modelCacheService = new();
        _modelSynchronizationServiceMock = new();
        _testObject = new ModelService(_modelSynchronizationServiceMock.Object, _modelCacheService.Object);
        await _testObject.Initialize();
    }

    #endregion
}