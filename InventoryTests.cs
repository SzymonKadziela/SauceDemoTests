using NUnit.Framework;
using Microsoft.Playwright.NUnit;
using SauceDemoTests.PageObjects;

namespace SauceDemoTests;

public class InventoryTests : PageTest
{
    private LoginPage _loginPage = null!;
    private InventoryPage _inventoryPage = null!;

    [SetUp]
    public async Task SetUp()
    {
        _loginPage = new LoginPage(Page);
        _inventoryPage = new InventoryPage(Page);

        // Każdy test zaczyna od zalogowania
        await _loginPage.GoToAsync();
        await _loginPage.LoginAsync("standard_user", "secret_sauce");
    }

    [Test]
    public async Task Inventory_WyswietlaSzescProduktow()
    {
        var count = await _inventoryPage.GetProductCountAsync();
        Assert.That(count, Is.EqualTo(6));
    }

    [Test]
    public async Task DodanieDoKoszyka_AktualizujeLicznik()
    {
        await _inventoryPage.AddToCartAsync("Sauce Labs Backpack");

        var count = await _inventoryPage.GetCartCountAsync();
        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public async Task DodanieTrzechProduktow_LicznikRownyTrzem()
    {
        await _inventoryPage.AddToCartAsync("Sauce Labs Backpack");
        await _inventoryPage.AddToCartAsync("Sauce Labs Bike Light");
        await _inventoryPage.AddToCartAsync("Sauce Labs Bolt T-Shirt");

        var count = await _inventoryPage.GetCartCountAsync();
        Assert.That(count, Is.EqualTo(3));
    }

    [Test]
    public async Task Sortowanie_AZ_ProduktySaPoKolei()
    {
        await _inventoryPage.SortByAsync("az");

        var names = await _inventoryPage.GetAllProductNamesAsync();
        var sorted = names.OrderBy(n => n).ToList();

        Assert.That(names, Is.EqualTo(sorted));
    }
}