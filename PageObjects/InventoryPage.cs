using Microsoft.Playwright;

namespace SauceDemoTests.PageObjects;

public class InventoryPage
{
    private readonly IPage _page;

    private ILocator ProductItems => _page.Locator(".inventory_item");
    private ILocator CartBadge => _page.Locator(".shopping_cart_badge");
    private ILocator CartIcon => _page.Locator(".shopping_cart_link");
    private ILocator SortDropdown => _page.Locator("[data-test='product-sort-container']");
    private ILocator ProductNames => _page.Locator(".inventory_item_name");

    public InventoryPage(IPage page)
    {
        _page = page;
    }

    public async Task<int> GetProductCountAsync()
        => await ProductItems.CountAsync();

    public async Task AddToCartAsync(string productName)
    {
        var item = _page.Locator(".inventory_item")
            .Filter(new() { HasText = productName });
        await item.Locator("button").ClickAsync();
    }

    public async Task<int> GetCartCountAsync()
    {
        if (!await CartBadge.IsVisibleAsync())
            return 0;
        var text = await CartBadge.TextContentAsync();
        return int.TryParse(text, out var count) ? count : 0;
    }

    public async Task GoToCartAsync()
        => await CartIcon.ClickAsync();

    public async Task SortByAsync(string option)
        => await SortDropdown.SelectOptionAsync(option);

    public async Task<List<string>> GetAllProductNamesAsync()
    {
        var names = new List<string>();
        var count = await ProductNames.CountAsync();
        for (int i = 0; i < count; i++)
            names.Add(await ProductNames.Nth(i).TextContentAsync() ?? string.Empty);
        return names;
    }
}