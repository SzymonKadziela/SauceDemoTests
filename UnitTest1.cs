using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SauceDemoTests;

public class Tests : PageTest
{
    [Test]
    public async Task OtworzStrone()
    {
        await Page.GotoAsync("https://www.saucedemo.com");
        
        await Expect(Page).ToHaveTitleAsync("Swag Labs");
    }
}