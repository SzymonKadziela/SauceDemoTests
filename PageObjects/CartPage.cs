using Microsoft.Playwright;

namespace SauceDemoTests.PageObjects;

public class CartPage
{
    private readonly IPage _page;

    private ILocator CartItems => _page.Locator(".cart_item");
    private ILocator CheckoutButton => _page.Locator("[data-test='checkout']");
    private ILocator ContinueShoppingButton => _page.Locator("[data-test='continue-shopping']");
    private ILocator ItemNames => _page.Locator(".inventory_item_name");
    private ILocator ItemPrice => _page.Locator(".inventory_item_price");

    public CartPage(IPage page)
    {
        _page = page;
    }

    public async Task<int> GetItemCountAsync()
        => await CartItems.CountAsync();

    public async Task<bool> IsItemInCartAsync(string productName)
    {
        var names = await ItemNames.AllTextContentsAsync();
        return names.Contains(productName);
    }

    public async Task GoToCheckoutAsync()
        => await CheckoutButton.ClickAsync();

    public async Task ContinueShoppingAsync()
        => await ContinueShoppingButton.ClickAsync();

    public async Task<double> GetProductPriceAsync(string productName)
    {
        var itemPrice = _page.Locator(".cart_item")
            .Filter(new() { HasText = productName });

        var priceText = await itemPrice.Locator(".inventory_item_price").TextContentAsync() ?? "0";
        var cleaned = priceText.Replace("$", "");

        return double.Parse(cleaned,
            System.Globalization.CultureInfo.InvariantCulture);
    }

}