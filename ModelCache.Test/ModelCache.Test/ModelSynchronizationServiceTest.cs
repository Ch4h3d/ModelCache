using ModelCache.Api;
using ModelCache.Services;
using ModelCache.Test.TestModels;
using Moq;

namespace ModelCache.Test;

public class ModelSynchronizationServiceTest
{
    private Mock<IServiceProvider> _serviceProviderServiceMock;
    private Mock<TestService> _testServiceMock;

    private ModelSynchronizationService _testObject;
    private readonly Dictionary<Guid, TestModel> _testModels = new();

    #region SimpleTest

    [Fact]
    public async Task TestLoadEffectsViaReflection()
    {
        await Setup();
        _testServiceMock.Setup(m => m.Ping());
        _testObject.ScanAssemblies(typeof(Effects).Assembly);
        await _testObject.CreateModel(new TestModel(Guid.NewGuid(), "Test"));
        
        _testServiceMock.Verify(m => m.Ping(), Times.Once);

    }
    
    #endregion

    #region Setup

    private async Task Setup()
    {
        _serviceProviderServiceMock = new();
        _testServiceMock = new();
        _serviceProviderServiceMock.Setup(m => m.GetService(typeof(TestService))).Returns(_testServiceMock.Object);
        _testObject = new ModelSynchronizationService(_serviceProviderServiceMock.Object);
        await _testObject.Initialize();
    }

    #endregion
}