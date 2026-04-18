using NUnit.Framework;
using Microsoft.Playwright.NUnit;
using SauceDemoTests.PageObjects;
using SauceDemoTests.Configuration;
using System.Text.RegularExpressions;

namespace SauceDemoTests;

public class InventoryTests : BaseTest
{
    private LoginPage _loginPage = null!;
    private InventoryPage _inventoryPage = null!;
    private CartPage _cartPage = null!;

    [SetUp]
    public async Task SetUp()
    {
        _loginPage = new LoginPage(Page);
        _inventoryPage = new InventoryPage(Page);
        _cartPage = new CartPage(Page);
        
        await _loginPage.GoToAsync();
    }

    [Test]
    public async Task Inventory_WyswietlaSzescProduktow()
    {
        await LoginAs("Standard");
        var count = await _inventoryPage.GetProductCountAsync();
        Assert.That(count, Is.EqualTo(6));
    }

    [Test]
    public async Task DodanieDoKoszyka_AktualizujeLicznik()
    {
        await LoginAs("Standard");
        await _inventoryPage.AddToCartAsync("Sauce Labs Backpack");
        var count = await _inventoryPage.GetCartCountAsync();
        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public async Task DodanieTrzechProduktow_LicznikRownyTrzem()
    {
        await LoginAs("Standard");
        await _inventoryPage.AddToCartAsync("Sauce Labs Backpack");
        await _inventoryPage.AddToCartAsync("Sauce Labs Bike Light");
        await _inventoryPage.AddToCartAsync("Sauce Labs Bolt T-Shirt");
        var count = await _inventoryPage.GetCartCountAsync();
        Assert.That(count, Is.EqualTo(3));
    }

    [Test]
    public async Task Sortowanie_AZ_ProduktySaPoKolei()
    {
        await LoginAs("Standard");
        await _inventoryPage.SortByAsync("az");
        var names = await _inventoryPage.GetAllProductNamesAsync();
        var sorted = names.OrderBy(n => n).ToList();
        Assert.That(names, Is.EqualTo(sorted));
    }

    [Test]
    public async Task Sortowanie_ZA_ProduktySaOdwrotnie()
    {
        await LoginAs("Standard");
        await _inventoryPage.SortByAsync("za");
        var names = await _inventoryPage.GetAllProductNamesAsync();
        var sorted = names.OrderByDescending(n => n).ToList();
        Assert.That(names, Is.EqualTo(sorted));
    }

    [Test]
    public async Task Sortowanie_CenaRosnaco_ProduktySaPoKolei()
    {
        await LoginAs("Standard");
        await _inventoryPage.SortByAsync("lohi");
        var prices = await _inventoryPage.GetAllProductPricesAsync();
        var sorted = prices.OrderBy(p => p).ToList();
        Assert.That(prices, Is.EqualTo(sorted));
    }

    [Test]
    public async Task Sortowanie_CenaMalejaco_ProduktySaPoKolei()
    {
        await LoginAs("Standard");
        await _inventoryPage.SortByAsync("hilo");
        var prices = await _inventoryPage.GetAllProductPricesAsync();
        var sorted = prices.OrderByDescending(p => p).ToList();
        Assert.That(prices, Is.EqualTo(sorted));
    }

    [Test]
    public async Task UsuniecieZKoszyka_AktualizujeLicznik()
    {
        await LoginAs("Standard");
        await _inventoryPage.AddToCartAsync("Sauce Labs Backpack");
        Assert.That(await _inventoryPage.GetCartCountAsync(), Is.EqualTo(1));

        await _inventoryPage.RemoveFromCartAsync("Sauce Labs Backpack");
        Assert.That(await _inventoryPage.GetCartCountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task CenyProduktow_SaWiekszeDZero()
    {
        await LoginAs("Standard");
        var prices = await _inventoryPage.GetAllProductPricesAsync();
        Assert.That(prices.Count, Is.EqualTo(6));
        Assert.That(prices.All(p => p > 0), Is.True);
    }

    [Test]
    public async Task ProductDetails_TitleAndPrice_MatchCatalog()
    {
        await LoginAs("Standard");
        var products = await _inventoryPage.GetAllProductNamesAsync();
        var productName = products[0];

        await _inventoryPage.ClickProductByNameAsync(productName);
        await Expect(Page.Locator(".inventory_details_name"))
        .ToHaveTextAsync(productName);

        var priceText = await Page.Locator(".inventory_details_price").InnerTextAsync();
        var price = decimal.Parse(
            priceText.Replace("$", ""),
            System.Globalization.CultureInfo.InvariantCulture);
        Assert.That(price, Is.GreaterThan(0));

    }

    [Test]
    public async Task ContinueShopping_AfterAddingProduct_CartItemsArePreserved()
    {
        await LoginAs("Standard");
        await _inventoryPage.AddToCartAsync("Sauce Labs Backpack");
        await _inventoryPage.GoToCartAsync();
        await _cartPage.ContinueShoppingAsync();
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
        var count = await _inventoryPage.GetCartCountAsync();
        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public async Task Verifies_product_count_matches_cart()
    {
        await LoginAs("Standard");
        await _inventoryPage.AddManyToCartAsync(
            "Sauce Labs Backpack", 
            "Sauce Labs Bike Light", 
            "Sauce Labs Onesie"
        );
        var icount = await _inventoryPage.GetCartCountAsync();
        Assert.That(icount, Is.EqualTo(3));
        await _inventoryPage.GoToCartAsync();
        var ccount = await _cartPage.GetItemCountAsync();
        Assert.That(ccount, Is.EqualTo(3));
    }

    [Test]
    public async Task SortByPriceLowToHigh_CheapestProduct_PriceMatchesInCart()
    {
        await LoginAs("Standard");
        await _inventoryPage.SortByAsync("lohi");
        var allPrice = await _inventoryPage.GetAllProductPricesAsync();
        var allNames = await _inventoryPage.GetAllProductNamesAsync();
        await _inventoryPage.AddToCartAsync(allNames[0]);
        await _inventoryPage.GoToCartAsync();
        var count = await _cartPage.GetItemCountAsync();
        Assert.That(count, Is.EqualTo(1));
        var lowPriceCart = await _cartPage.GetProductPriceAsync(allNames[0]);
        var lowPriceInventory = allPrice[0];
        Assert.That(lowPriceCart, Is.EqualTo(lowPriceInventory));
    }

    [Test]
    public async Task Verifies_description_isNotEmpty()
    {
        await LoginAs("Standard");
        var products = await _inventoryPage.GetAllProductNamesAsync();
        await _inventoryPage.ClickProductByNameAsync(products[0]);
        var productDesc = await Page.Locator(".inventory_details_desc").TextContentAsync();
        Assert.That(productDesc, Is.Not.Empty);
        Assert.That(productDesc, Has.Length.GreaterThan(10));
    }

    [Test]
    public async Task Verifies_problemUser_haveBrokenImages()
    {
        await LoginAs("Problem");
        var images = await Page.Locator(".inventory_item_img img").AllAsync();
        var srcList = await Task.WhenAll(images.Select(img => img.GetAttributeAsync("src")));

        Assert.That(srcList, Is.All.Contains("sl-404"));
        Assert.That(srcList.Distinct().Count(), Is.EqualTo(1), 
                "Użytkownik powinien widzieć tylko jeden i ten sam typ obrazka!");
        
    }
    
}