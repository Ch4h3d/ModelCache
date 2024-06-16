using ModelCache.Api;

namespace ModelCache.Test.TestModels;

public class TestModel : IModel
{
    public TestModel(Guid id, string testString = "")
    {
        Id = id;
        TestString = testString;
    }

    public Guid Id { get; }
    
    public string TestString { get; }
}