using ModelCache.Api.Attributes;
using ModelCache.Api.Models;
using ModelCache.Test.TestModels;

namespace ModelCache.Test;

public class Effects
{
    [CreateEffect(typeof(TestModel))]
    public static async Task<object?> TestCreateEffect(IServiceProvider serviceProvider, object model)
    {
        var service = serviceProvider.GetService(typeof(TestService)) as TestService;
        service?.Ping();
        return await Task.FromResult(model);
    }
    
    [ReadEffect(typeof(TestModel))]
    public static async Task<object?> TestReadEffect(IServiceProvider serviceProvider, Guid modelId, object[] parameters)
    {
        return await Task.FromResult(new TestModel(modelId, "ReadFromEffect"));
    }
    
    [ReadEffect(typeof(TestModel))]
    public static async Task<object[]> TestReadManyEffect(IServiceProvider serviceProvider, ModelSearchOptionsBase searchOptionsBase, Dictionary<string, object> dict)
    {
        return await Task.FromResult(Array.Empty<object>());
    }
    
    [ReadEffect(typeof(TestModel))]
    public static IQueryable<object> TestReadManyFromCacheEffect(IServiceProvider serviceProvider, ModelSearchOptionsBase searchOptionsBase, Dictionary<string, object> dict)
    {
        return Array.Empty<object>().AsQueryable();
    }
    
    [UpdateEffect(typeof(TestModel))]
    public static async Task<object?> TestUpdateEffect(IServiceProvider serviceProvider, object model)
    {
        return await Task.FromResult(model);
    }
    
    [DeleteEffect(typeof(TestModel))]
    public static async Task TestDeleteEffect(IServiceProvider serviceProvider, Guid modelId)
    {
        await Task.CompletedTask;
    }
}