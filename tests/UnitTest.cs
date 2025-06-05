using Xunit;

namespace DotnetDemoapp_tests;

public class LameUnitTest1
{
    [Fact]
    public void TestAThingFalse()
    {
        bool result = false;
        Assert.False(result, $"{result} should not be true");
    }
}

public class LameUnitTest2
{
    [Fact]
    public void TestAThingTrue()
    {
        bool result = true;
        Assert.True(result, $"{result} should not be false");
    }
}

public class ApiHelperTests
{
    [Fact]
    public async Task GetCpuUsageForProcess_ReturnsValidPercentage()
    {
        // Act
        var result = await DotnetDemoapp.ApiHelper.GetCpuUsageForProcess();
        
        // Assert
        Assert.True(result >= 0);
        Assert.True(result <= 100);
    }
}
