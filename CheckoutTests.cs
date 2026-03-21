using NUnit.Framework;
using Microsoft.Playwright.NUnit;
using SauceDemoTests.PageObjects;

namespace SauceDemoTests;

public class CheckoutTests : PageTest
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

        // Sprawdź czy wróciliśmy do koszyka
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
}