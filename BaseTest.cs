using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SauceDemoTests.Configuration;
using SauceDemoTests.PageObjects;

namespace SauceDemoTests;

public class BaseTest : PageTest
{
    protected TestDataConfig Config { get; private set; } = null!;

    [SetUp]
    public void BaseSetup()
    {
        Config = ConfigReader.GetConfig();
    }

    protected async Task LoginAs(string userKey) 
    {
        var user = Config.Users[userKey];
        
        var loginPage = new LoginPage(Page, Config.BaseUrl);
        await loginPage.GoToAsync();
        await loginPage.LoginAsync(user.Username, user.Password);
    }
        
    [TearDown]
    public async Task TakeScreenshotOnFailure()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var screenshotPath = $"screenshots/{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";

            Directory.CreateDirectory("screenshots");

            await Page.ScreenshotAsync(new()
            {
                Path = screenshotPath,
                FullPage = true
            });

            TestContext.Out.WriteLine($"Screenshot saved: {screenshotPath}");
        }
    }
}