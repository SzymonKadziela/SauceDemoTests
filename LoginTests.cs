using NUnit.Framework;
using Microsoft.Playwright.NUnit;
using SauceDemoTests.PageObjects;
using SauceDemoTests.Configuration;
using System.Text.RegularExpressions;

namespace SauceDemoTests;

public class LoginTests : BaseTest
{
    private LoginPage _loginPage = null!;
    private InventoryPage _inventoryPage = null!;

    [SetUp]
    public async Task SetUp()
    {
        _loginPage = new LoginPage(Page);
        _inventoryPage = new InventoryPage(Page);
        await _loginPage.GoToAsync();
    }

    [Test]
    [TestCaseSource(typeof(UserFactory), nameof(UserFactory.GetLoginTestData))]
    public async Task PoprawneLogowanie_PrzenosiBDoInventory(UserCredentials user)
    {
        await _loginPage.LoginAsync(user.Username, user.Password);
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
    }

    [Test]
    [TestCaseSource(typeof(UserFactory), nameof(UserFactory.GetLoginTestData))]
    public async Task BledneHaslo_WyswietlaBlad(UserCredentials user)
    {
        await _loginPage.LoginAsync(user.Username, "zle_haslo");
        var errorVisible = await _loginPage.IsErrorVisibleAsync();
        Assert.That(errorVisible, Is.True);
    }

    [Test]
    public async Task ZablokowanyUzytkownik_WyswietlaBlad()
    {
        await _loginPage.LoginAsync("locked_out_user", "secret_sauce");
        var blad = await _loginPage.GetErrorMessageAsync();
        Assert.That(blad, Does.Contain("locked out"));
    }

    [TestCase("problem_user", "secret_sauce", TestName = "ProblemUser_MozeWejscNaInventory")]
    [TestCase("visual_user", "secret_sauce", TestName = "VisualUser_MozeWejscNaInventory")]
    [TestCase("performance_glitch_user", "secret_sauce", TestName = "PerformanceUser_MozeWejscNaInventory")]
    [TestCase("error_user", "secret_sauce", TestName = "ErrorUser_MozeWejscNaInventory")]
    public async Task RozniUzytkownicy_MogaSieZalogowac(string username, string password)
    {
        await _loginPage.LoginAsync(username, password);
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
    }

    [Test]
    public async Task BlockEntryToInventoryWithoutLogin()
    {
        await _loginPage.LoginAsync("standard_user", "secret_sauce");
        await _loginPage.LogoutAsync();
        await Page.GotoAsync("https://www.saucedemo.com/inventory.html");
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com");
    }

    [Test]
    public async Task LoginError_AfterClickingX_ErrorMessageDisappears()
    {
        await _loginPage.LoginAsync("standard_userError", "secret_sauce");
        var errorVisible = await _loginPage.IsErrorVisibleAsync();
        Assert.That(errorVisible, Is.True);
        await _loginPage.CloseErrorAsync();
        var errorNotVisible = await _loginPage.IsErrorVisibleAsync();
        Assert.That(errorNotVisible, Is.False);
    }
}