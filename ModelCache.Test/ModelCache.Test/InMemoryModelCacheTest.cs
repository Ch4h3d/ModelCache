using ModelCache.Api;
using ModelCache.Services;
using ModelCache.Test.TestModels;

namespace ModelCache.Test;

public class InMemoryModelCacheTest
{
    private const int NumModels = 5;

    private IModelCacheService _testObject;
    private readonly Dictionary<Guid, TestModel> _testModels = new();

    #region SimpleTest

    //  Tests for basic method calls.

    /// <summary>
    ///     Stores a single model and check retrieval.
    ///     Tests <see cref="InMemoryModelCache.StoreModel{T}(T,bool,bool)"/>
    /// </summary>
    [Fact]
    public void TestSingleModelStore()
    {
        Setup(false);
        
        Assert.Empty(_testObject.RetrieveModels<TestModel>());

        var n = 5;
        Dictionary<Guid, TestModel> testModels = new();
        for (var i = 0; i < n; i++)
        {
            var guid = Guid.NewGuid();
            testModels.Add(guid, new TestModel(guid));
        }

        foreach (var model in testModels.Values)
        {
            _testObject.StoreModel(model);
        }

        foreach (var id in testModels.Keys)
        {
            Assert.True(_testObject.ModelExists<TestModel>(id));
            var modelFromCache = _testObject.RetrieveModel<TestModel>(id);
            Assert.NotNull(modelFromCache);
            Assert.Equal(id, modelFromCache.Id);
        }
    }

    /// <summary>
    ///     Stores several models and checks retrieval.
    ///     Tests <see cref="InMemoryModelCache.StoreModels(IEnumerable{T},bool,bool)"/>
    /// </summary>
    [Fact]
    public void TestManyModelStore()
    {
        Setup();

        var modelsFromCache = _testObject.RetrieveModels<TestModel>();
        Assert.Equal(NumModels, modelsFromCache.Count());

        foreach (var id in _testModels.Keys)
        {
            Assert.True(_testObject.ModelExists<TestModel>(id));
            Assert.Contains(id, modelsFromCache.Select(m => m.Id));
        }
    }


    [Fact]
    public void TestModelUpdate()
    {
        Setup();
        var idOne = _testModels.Keys.First();
        var modelOne = _testObject.RetrieveModel<TestModel>(idOne);
        Assert.NotNull(modelOne);
        Assert.Empty(modelOne.TestString);

        const string testString = "Test";
        _testObject.StoreModel(new TestModel(modelOne.Id, testString));

        modelOne = _testObject.RetrieveModel<TestModel>(idOne);
        Assert.NotNull(modelOne);
        Assert.Equal(testString, modelOne.TestString);

        var idTwo = _testModels.Keys.Skip(1).First();
        var modelTwo = _testObject.RetrieveModel<TestModel>(idTwo);
        Assert.NotNull(modelTwo);
        Assert.Empty(modelTwo.TestString);

        _testObject.StoreModel(new TestModel(modelTwo.Id, testString));

        modelTwo = _testObject.RetrieveModel<TestModel>(idTwo);
        Assert.NotNull(modelTwo);
        Assert.Equal(testString, modelTwo.TestString);

        modelOne = _testObject.RetrieveModel<TestModel>(idOne);
        Assert.NotNull(modelOne);
        Assert.Equal(testString, modelOne.TestString);
    }


    [Fact]
    public void TestModelDelete()
    {
        Setup();
        var idOne = _testModels.Keys.First();
        var modelOne = _testObject.RetrieveModel<TestModel>(idOne);
        Assert.NotNull(modelOne);
        Assert.Empty(modelOne.TestString);

        modelOne = _testObject.RetrieveModel<TestModel>(idOne);
        Assert.NotNull(modelOne);

        _testObject.DeleteModel<TestModel>(idOne);

        modelOne = _testObject.RetrieveModel<TestModel>(idOne);
        Assert.Null(modelOne);


        var modelsFromCache = _testObject.RetrieveModels<TestModel>();
        Assert.Equal(NumModels - 1, modelsFromCache.Count());
        Assert.True(modelsFromCache.All(m => string.IsNullOrWhiteSpace(m.TestString)));
    }

    /// <summary>
    ///     Tests the element valid function.
    /// </summary>
    [Fact]
    public void TestModelValidity()
    {
        Setup();
        Assert.False(_testObject.AreCacheElementsValid<TestModel>(new []{Guid.Empty}));
        // Recently added models should be valid
        Assert.True(_testObject.AreCacheElementsValid<TestModel>(_testModels.Keys));
        Assert.True(_testObject.AreCacheElementsValid(_testModels.Values));
    }

    #endregion

    #region Setup

    private void Setup(bool addTestModels = true)
    {
        _testObject = new InMemoryModelCache();

        if (!addTestModels) return;
        
        for (var i = 0; i < NumModels; i++)
        {
            var guid = Guid.NewGuid();
            _testModels.Add(guid, new TestModel(guid));
        }

        _testObject.StoreModels(_testModels.Values);
    }

    #endregion
}