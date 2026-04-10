using Microsoft.Playwright;

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
}