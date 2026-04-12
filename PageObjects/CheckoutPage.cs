using Microsoft.Playwright;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SauceDemoTests.PageObjects;

public class CheckoutPage
{
    private readonly IPage _page;

    private ILocator FirstNameInput => _page.Locator("[data-test='firstName']");
    private ILocator LastNameInput => _page.Locator("[data-test='lastName']");
    private ILocator PostalCodeInput => _page.Locator("[data-test='postalCode']");
    private ILocator ContinueButton => _page.Locator("[data-test='continue']");
    private ILocator FinishButton => _page.Locator("[data-test='finish']");
    private ILocator ConfirmationHeader => _page.Locator(".complete-header");
    private ILocator ErrorMessage => _page.Locator("[data-test='error']");
    private ILocator CancelButton => _page.Locator("[data-test='cancel']");
    private ILocator BackHomeButton => _page.Locator("[data-test='back-to-products']");
    private ILocator ItemTotal => _page.Locator(".summary_subtotal_label");
    private ILocator Tax => _page.Locator(".summary_tax_label");
    private ILocator Total => _page.Locator(".summary_total_label");

    public CheckoutPage(IPage page)
    {
        _page = page;
    }

    public async Task FillShippingInfoAsync(string firstName, string lastName, string postalCode)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await PostalCodeInput.FillAsync(postalCode);
    }

    public async Task ClickContinueAsync()
        => await ContinueButton.ClickAsync();

    public async Task ClickFinishAsync()
        => await FinishButton.ClickAsync();

    public async Task ClickCancelAsync()
        => await CancelButton.ClickAsync();

    public async Task<string> GetConfirmationHeaderAsync()
        => await ConfirmationHeader.TextContentAsync() ?? string.Empty;

    public async Task<bool> IsOrderSuccessfulAsync()
    {
        var header = await GetConfirmationHeaderAsync();
        return header.Contains("Thank you");
    }

    public async Task<string> GetErrorMessageAsync()
        => await ErrorMessage.TextContentAsync() ?? string.Empty;

    public async Task<bool> IsErrorVisibleAsync()
        => await ErrorMessage.IsVisibleAsync();

    public async Task ClickBackHomeAsync()
        => await BackHomeButton.ClickAsync();

    public async Task CompletePurchaseAndReturnHomeAsync(string firstName, string lastName, string zip)
    {
        await FillShippingInfoAsync(firstName, lastName, zip);
        await ContinueButton.ClickAsync();
        await FinishButton.ClickAsync();
        await BackHomeButton.ClickAsync();
    }

    private async Task<decimal> ParsePriceAsync(ILocator locator)
    {
       var text = await locator.InnerTextAsync();
       var match = Regex.Match(text, @"\d+\.\d+");

        if (match.Success)
        {
            return decimal.Parse(match.Value, CultureInfo.InvariantCulture);
        }

        throw new Exception($"Błąd: Nie znaleziono kwoty w tekście: '{text}'");
    }

    public async Task<decimal> GetSubtotalAsync() => await ParsePriceAsync(ItemTotal);
    public async Task<decimal> GetTaxAsync() => await ParsePriceAsync(Tax);
    public async Task<decimal> GetTotalAsync() => await ParsePriceAsync(Total);
}