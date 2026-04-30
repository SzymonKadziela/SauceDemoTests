using Microsoft.Playwright;

namespace SauceDemoTests.PageObjects;

public class LoginPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    // Lokatory - czyli "adresy" elementów na stronie
    private ILocator UsernameInput => _page.Locator("#user-name");
    private ILocator PasswordInput => _page.Locator("#password");
    private ILocator LoginButton => _page.Locator("#login-button");
    private ILocator ErrorMessage => _page.Locator("[data-test='error']");
    private ILocator BurgerMenu => _page.Locator("#react-burger-menu-btn");
    private ILocator LogoutButton => _page.Locator("#logout_sidebar_link");
    private ILocator ErrorButton => _page.Locator(".error-button");
    private ILocator ResetAppState => _page.Locator("[data-test='reset-sidebar-link']");

    public LoginPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    // Akcje które można wykonać na stronie logowania
    public async Task GoToAsync()
        => await _page.GotoAsync(_baseUrl);

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

    public async Task LogoutAsync()
    {
        await BurgerMenu.ClickAsync();
        await LogoutButton.WaitForAsync();
        await LogoutButton.ClickAsync();
    }

    public async Task CloseErrorAsync()
        => await ErrorButton.ClickAsync();

    public async Task ResetAppStateAsync()
    {
        await BurgerMenu.ClickAsync();
        await ResetAppState.WaitForAsync();
        await ResetAppState.ClickAsync();
    }
}