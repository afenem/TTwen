using Microsoft.Playwright;

namespace TTwen.Infrastructure.Browser;

/// <summary>
/// Playwright tarayıcı oturumunun yaşam döngüsünü yönetir.
/// </summary>
/// <remarks>
/// Tarayıcıyı başlatıp kapatmak ile TTWars selector kodunu ayırır. Bu sınıf
/// sadece tarayıcı altyapısından sorumludur.
/// </remarks>
public sealed class PlaywrightBrowserSession : IAsyncDisposable
{
    private IPlaywright? _playwright;
    private IBrowserContext? _context;

    public IPage? CurrentPage { get; private set; }

    public async Task StartAsync(string userDataDirectory)
    {
        _playwright = await Playwright.CreateAsync();

        _context = await _playwright.Chromium.LaunchPersistentContextAsync(
            userDataDirectory,
            new BrowserTypeLaunchPersistentContextOptions
            {
                Headless = false,
                ViewportSize = new ViewportSize { Width = 1440, Height = 900 }
            });

        CurrentPage = _context.Pages.FirstOrDefault()
            ?? await _context.NewPageAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_context is not null)
            await _context.DisposeAsync();

        _playwright?.Dispose();
    }
}
