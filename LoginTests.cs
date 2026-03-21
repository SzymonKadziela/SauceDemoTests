using NUnit.Framework;
using Microsoft.Playwright.NUnit;
using SauceDemoTests.PageObjects;

namespace SauceDemoTests;

public class LoginTests : PageTest
{
    private LoginPage _loginPage = null!;

    [SetUp]
    public async Task SetUp()
    {
        _loginPage = new LoginPage(Page);
        await _loginPage.GoToAsync();
    }

    [Test]
    public async Task PoprawneLogowanie_PrzenosiBDoInventory()
    {
        await _loginPage.LoginAsync("standard_user", "secret_sauce");

        // Sprawdzamy czy URL zmienił się na inventory
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
    }

    [Test]
    public async Task PoprawneLogowanie_problemuser()
    {
        await _loginPage.LoginAsync("problem_user", "secret_sauce");
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
    }


    [Test]
    public async Task BledneHaslo_WyswietlaBlad()
    {
        await _loginPage.LoginAsync("standard_user", "zle_haslo");

        var czyBladWidoczny = await _loginPage.IsErrorVisibleAsync();
        Assert.That(czyBladWidoczny, Is.True);
    }

    [Test]
    public async Task ZablokowanyUzytkownik_WyswietlaBlad()
    {
        await _loginPage.LoginAsync("locked_out_user", "secret_sauce");

        var blad = await _loginPage.GetErrorMessageAsync();
        Assert.That(blad, Does.Contain("locked out"));
    }

    
}