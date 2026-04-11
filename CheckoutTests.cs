using NUnit.Framework;
using Microsoft.Playwright.NUnit;
using SauceDemoTests.PageObjects;
using Microsoft.VisualBasic;

namespace SauceDemoTests;

public class CheckoutTests : BaseTest
{
    private LoginPage _loginPage = null!;
    private InventoryPage _inventoryPage = null!;
    private CartPage _cartPage = null!;
    private CheckoutPage _checkoutPage = null!;

    [SetUp]
    public async Task SetUp()
    {
        _loginPage = new LoginPage(Page);
        _inventoryPage = new InventoryPage(Page);
        _cartPage = new CartPage(Page);
        _checkoutPage = new CheckoutPage(Page);

        await _loginPage.GoToAsync();
        await _loginPage.LoginAsync("standard_user", "secret_sauce");
    }

    [Test]
    public async Task PelnyCheckout_ZamowienieZakonczonePomyslnie()
    {
        // Dodaj produkt do koszyka
        await _inventoryPage.AddToCartAsync("Sauce Labs Backpack");
        await _inventoryPage.GoToCartAsync();

        // Sprawdź czy produkt jest w koszyku
        var isInCart = await _cartPage.IsItemInCartAsync("Sauce Labs Backpack");
        Assert.That(isInCart, Is.True);

        // Przejdź do checkout
        await _cartPage.GoToCheckoutAsync();

        // Wypełnij dane
        await _checkoutPage.FillShippingInfoAsync("Jan", "Kowalski", "00-001");
        await _checkoutPage.ClickContinueAsync();

        // Zakończ zamówienie
        await _checkoutPage.ClickFinishAsync();

        // Sprawdź potwierdzenie
        var isSuccess = await _checkoutPage.IsOrderSuccessfulAsync();
        Assert.That(isSuccess, Is.True);
    }

    [Test]
    public async Task Checkout_BrakImienia_WyswietlaBlad()
    {
        await _inventoryPage.AddToCartAsync("Sauce Labs Backpack");
        await _inventoryPage.GoToCartAsync();
        await _cartPage.GoToCheckoutAsync();

        // Brak imienia
        await _checkoutPage.FillShippingInfoAsync("", "Kowalski", "00-001");
        await _checkoutPage.ClickContinueAsync();

        var isError = await _checkoutPage.IsErrorVisibleAsync();
        Assert.That(isError, Is.True);

        var errorMsg = await _checkoutPage.GetErrorMessageAsync();
        Assert.That(errorMsg, Does.Contain("First Name is required"));
    }

    [Test]
    public async Task Checkout_AnulowanieWracaDoKoszyka()
    {
        await _inventoryPage.AddToCartAsync("Sauce Labs Backpack");
        await _inventoryPage.GoToCartAsync();
        await _cartPage.GoToCheckoutAsync();

        await _checkoutPage.ClickCancelAsync();

        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/cart.html");
    }

    [Test]
    public async Task Checkout_WszystkieSzescProduktow_ZamowienieZakonczonePomyslnie()
    {
        // Dodaj wszystkie 6 produktów do koszyka
        var names = await _inventoryPage.GetAllProductNamesAsync();
        foreach (var name in names)
            await _inventoryPage.AddToCartAsync(name);

        // Sprawdź czy wszystkie są w koszyku
        var cartCount = await _inventoryPage.GetCartCountAsync();
        Assert.That(cartCount, Is.EqualTo(6));

        // Przejdź przez checkout
        await _inventoryPage.GoToCartAsync();
        await _cartPage.GoToCheckoutAsync();
        await _checkoutPage.FillShippingInfoAsync("Jan", "Kowalski", "00-001");
        await _checkoutPage.ClickContinueAsync();
        await _checkoutPage.ClickFinishAsync();

        // Sprawdź potwierdzenie
        var isSuccess = await _checkoutPage.IsOrderSuccessfulAsync();
        Assert.That(isSuccess, Is.True);
    }

    [Test]
    public async Task Add_product_to_checkout_and_verify()
    {
        await _inventoryPage.AddToCartAsync("Sauce Labs Fleece Jacket");
        await _inventoryPage.GoToCartAsync();

        var isInCart = await _cartPage.IsItemInCartAsync("Sauce Labs Fleece Jacket");
        Assert.That(isInCart, Is.True);

        var itemCount = await _cartPage.GetItemCountAsync();
        Assert.That(itemCount, Is.EqualTo(1));
    }

    [Test]
    public async Task Price_verify_checkout_vs_page()
    {
        var expectedPrice = await _inventoryPage.GetProductPriceByNameAsync("Sauce Labs Backpack");
        await _inventoryPage.AddToCartAsync("Sauce Labs Backpack");
        await _inventoryPage.GoToCartAsync();   
        var productPrice = await _cartPage.GetProductPriceAsync("Sauce Labs Backpack");
        Assert.That(productPrice, Is.EqualTo(expectedPrice));
    }

    [Test]
    public async Task Cart_badge_visibility_and_count()
    {
        await _inventoryPage.AddToCartAsync("Sauce Labs Backpack");
        await _inventoryPage.AddToCartAsync("Sauce Labs Bike Light");
        var badge = await _inventoryPage.GetCartCountAsync();
        Assert.That(badge, Is.EqualTo(2));
        await _inventoryPage.RemoveFromCartAsync("Sauce Labs Backpack");
        await _inventoryPage.RemoveFromCartAsync("Sauce Labs Bike Light");
        var count = await _inventoryPage.GetCartCountAsync();
        Assert.That(count, Is.EqualTo(0));
    }

    [Test]
    public async Task Checkout_AfterCompletion_BackHomeReturnsToInventoryWithEmptyCart()
    {
        await _inventoryPage.AddToCartAsync("Sauce Labs Backpack");
        await _inventoryPage.GoToCartAsync();
        await _cartPage.GoToCheckoutAsync();
        await _checkoutPage.CompletePurchaseAndReturnHomeAsync("John", "Doe", "21-377");
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
        var count = await _inventoryPage.GetCartCountAsync();
        Assert.That(count, Is.EqualTo(0));
    }

    [Test]
    public async Task RemoveAllItemsFromCart_CartIsEmpty_CheckoutButtonStillVisible()
    {
        var products = await _inventoryPage.GetAllProductNamesAsync();
        await _inventoryPage.AddManyToCartAsync(
            products[0], 
            products[1], 
            products[2]
        );
        await _inventoryPage.GoToCartAsync();
        await _cartPage.RemoveItemFromCartAsync(products[0]);
        await _cartPage.RemoveItemFromCartAsync(products[1]);
        await _cartPage.RemoveItemFromCartAsync(products[2]);
        var itemCounts = await _cartPage.GetItemCountAsync();
        Assert.That(itemCounts, Is.EqualTo(0));
        var isVisible = await _cartPage.IsCheckoutButtonVisibleAsync();
        Assert.That(isVisible, Is.True);
    }
}