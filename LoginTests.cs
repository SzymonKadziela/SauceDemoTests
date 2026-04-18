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
    public async Task BledneHaslo_WyswietlaBlad()
    {
        await _loginPage.LoginAsync(Config.Users["Standard"].Username, "zle_haslo");
        var errorVisible = await _loginPage.IsErrorVisibleAsync();
        Assert.That(errorVisible, Is.True);
    }

    [Test]
    public async Task ZablokowanyUzytkownik_WyswietlaBlad()
    {
        await _loginPage.LoginAsync(Config.Users["LockedOut"].Username, Config.Users["LockedOut"].Password);
        var blad = await _loginPage.GetErrorMessageAsync();
        Assert.That(blad, Does.Contain("locked out"));
    }

    [Test]
    [TestCaseSource(typeof(UserFactory), nameof(UserFactory.GetLoginTestData))]
    public async Task RozniUzytkownicy_MogaSieZalogowac(UserCredentials user)
    {
        await _loginPage.LoginAsync(user.Username, user.Password);
        await Expect(Page).ToHaveURLAsync(new Regex("inventory.html"));
    }

    [Test]
    public async Task BlockEntryToInventoryWithoutLogin()
    {
        await _loginPage.LoginAsync(Config.Users["Standard"].Username, Config.Users["Standard"].Password);
        await _loginPage.LogoutAsync();
        await Page.GotoAsync("https://www.saucedemo.com/inventory.html");
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com");
    }

    [Test]
    public async Task LoginError_AfterClickingX_ErrorMessageDisappears()
    {
        await _loginPage.LoginAsync(Config.Users["StandardError"].Username, Config.Users["Standard"].Password);
        var errorVisible = await _loginPage.IsErrorVisibleAsync();
        Assert.That(errorVisible, Is.True);
        await _loginPage.CloseErrorAsync();
        var errorNotVisible = await _loginPage.IsErrorVisibleAsync();
        Assert.That(errorNotVisible, Is.False);
    }
}