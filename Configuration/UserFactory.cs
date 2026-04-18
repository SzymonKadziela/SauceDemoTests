using NUnit.Framework;
using System.Collections.Generic;

namespace SauceDemoTests.Configuration
{
    public static class UserFactory
    {
        public static IEnumerable<TestCaseData> GetLoginTestData()
        {
            var config = ConfigReader.GetConfig();

            yield return new TestCaseData(config.Users["Standard"]).SetName("LoginTest_StandardUser");
            yield return new TestCaseData(config.Users["Problem"]).SetName("LoginTest_ProblemUser");
            yield return new TestCaseData(config.Users["Performance"]).SetName("LoginTest_PerformanceUser");
            yield return new TestCaseData(config.Users["ErrorUser"]).SetName("Login_ErrorUser");
            yield return new TestCaseData(config.Users["Visual"]).SetName("Login_VisualUser");
        }
    }
}