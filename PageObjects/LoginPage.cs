using Microsoft.Playwright;

namespace SauceDemoTests.PageObjects;

public class LoginPage
{
    private readonly IPage _page;

    // Lokatory - czyli "adresy" elementów na stronie
    private ILocator UsernameInput => _page.Locator("#user-name");
    private ILocator PasswordInput => _page.Locator("#password");
    private ILocator LoginButton => _page.Locator("#login-button");
    private ILocator ErrorMessage => _page.Locator("[data-test='error']");

    public LoginPage(IPage page)
    {
        _page = page;
    }

    // Akcje które można wykonać na stronie logowania
    public async Task GoToAsync()
        => await _page.GotoAsync("https://www.saucedemo.com");

    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public async Task<string> GetErrorMessageAsync()
        => await ErrorMessage.TextContentAsync() ?? string.Empty;

    public async Task<bool> IsErrorVisibleAsync()
        => await ErrorMessage.IsVisibleAsync();
}