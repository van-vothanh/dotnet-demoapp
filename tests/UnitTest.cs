using Xunit;

namespace DotnetDemoapp_tests
{
    /// <summary>
    /// Basic unit test class for testing false conditions
    /// </summary>
    public class LameUnitTest1
    {
        /// <summary>
        /// Tests that a false value is indeed false
        /// </summary>
        [Fact]
        public void TestAThingFalse()
        {
            bool result = false;
            Assert.False(result, $"{result} should not be true");
        }
    }

    /// <summary>
    /// Basic unit test class for testing true conditions
    /// </summary>
    public class LameUnitTest2
    {
        /// <summary>
        /// Tests that a true value is indeed true
        /// </summary>
        [Fact]
        public void TestAThingTrue()
        {
            bool result = true;
            Assert.True(result, $"{result} should not be false");
        }
    }
}
