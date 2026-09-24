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

    /// <summary>
    /// Aktif Playwright sayfasıdır.
    /// </summary>
    public IPage? CurrentPage { get; private set; }

    /// <summary>
    /// Kalıcı Chromium oturumunu başlatır.
    /// </summary>
    /// <param name="userDataDirectory">
    /// Tarayıcı profilinin tutulacağı TTwen'e özel klasör.
    /// </param>
    public async Task StartAsync(string userDataDirectory)
    {
        if (_context is not null)
        {
            CurrentPage ??= _context.Pages.Count > 0
                ? _context.Pages[0]
                : await _context.NewPageAsync();

            return;
        }

        if (string.IsNullOrWhiteSpace(userDataDirectory))
        {
            throw new ArgumentException(
                "Playwright kullanıcı veri klasörü boş olamaz.",
                nameof(userDataDirectory));
        }

        try
        {
            _playwright = await Playwright.CreateAsync();

            _context = await _playwright.Chromium.LaunchPersistentContextAsync(
                userDataDirectory,
                new BrowserTypeLaunchPersistentContextOptions
                {
                    Headless = false,
                    ViewportSize = new ViewportSize
                    {
                        Width = 1440,
                        Height = 900
                    }
                });

            CurrentPage = _context.Pages.Count > 0
                ? _context.Pages[0]
                : await _context.NewPageAsync();
        }
        catch
        {
            await DisposeAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_context is not null)
            await _context.DisposeAsync();

        _playwright?.Dispose();

        _context = null;
        _playwright = null;
        CurrentPage = null;
    }
}