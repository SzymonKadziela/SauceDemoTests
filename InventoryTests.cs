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

    [Test]
    public async Task Sortowanie_ZA_ProduktySaOdwrotnie()
    {
        await _inventoryPage.SortByAsync("za");
        var names = await _inventoryPage.GetAllProductNamesAsync();
        var sorted = names.OrderByDescending(n => n).ToList();
        Assert.That(names, Is.EqualTo(sorted));
    }

    [Test]
    public async Task Sortowanie_CenaRosnaco_ProduktySaPoKolei()
    {
        await _inventoryPage.SortByAsync("lohi");
        var prices = await _inventoryPage.GetAllProductPricesAsync();
        var sorted = prices.OrderBy(p => p).ToList();
        Assert.That(prices, Is.EqualTo(sorted));
    }

    [Test]
    public async Task Sortowanie_CenaMalejaco_ProduktySaPoKolei()
    {
        await _inventoryPage.SortByAsync("hilo");
        var prices = await _inventoryPage.GetAllProductPricesAsync();
        var sorted = prices.OrderByDescending(p => p).ToList();
        Assert.That(prices, Is.EqualTo(sorted));
    }

    [Test]
    public async Task UsuniecieZKoszyka_AktualizujeLicznik()
    {
        await _inventoryPage.AddToCartAsync("Sauce Labs Backpack");
        Assert.That(await _inventoryPage.GetCartCountAsync(), Is.EqualTo(1));

        await _inventoryPage.RemoveFromCartAsync("Sauce Labs Backpack");
        Assert.That(await _inventoryPage.GetCartCountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task CenyProduktow_SaWiekszeDZero()
    {
        var prices = await _inventoryPage.GetAllProductPricesAsync();
        Assert.That(prices.Count, Is.EqualTo(6));
        Assert.That(prices.All(p => p > 0), Is.True);
    }
}