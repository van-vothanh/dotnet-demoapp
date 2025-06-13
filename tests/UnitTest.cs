using Xunit;

namespace DotnetDemoapp_tests
{
    /// <summary>
    /// Basic unit tests for the .NET Demo Application
    /// These are simple placeholder tests to demonstrate the testing framework
    /// </summary>
    public class BasicUnitTests
    {
        /// <summary>
        /// Test to verify false assertion works correctly
        /// </summary>
        [Fact]
        public void TestFalseAssertion()
        {
            bool result = false;
            Assert.False(result, $"{result} should not be true");
        }

        /// <summary>
        /// Test to verify true assertion works correctly
        /// </summary>
        [Fact]
        public void TestTrueAssertion()
        {
            bool result = true;
            Assert.True(result, $"{result} should not be false");
        }
    }
}
