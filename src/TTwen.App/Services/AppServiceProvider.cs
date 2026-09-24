using TTwen.Application.Interfaces;
using TTwen.Infrastructure.Browser;
using TTwen.Infrastructure.TTWars;

namespace TTwen.App.Services;

/// <summary>
/// WinUI uygulamasının composition root'udur.
/// </summary>
/// <remarks>
/// Uygulama seviyesindeki somut Infrastructure bağımlılıklarını burada oluşturur.
/// Böylece Page sınıfları constructor içinde altyapı nesnelerini kendileri üretmez.
/// </remarks>
public sealed class AppServiceProvider : IAsyncDisposable
{
    /// <summary>
    /// TTWars bağlantı hizmetidir.
    /// </summary>
    public ITTWarsConnectionService TTWarsConnection { get; }

    /// <summary>
    /// Playwright tarayıcı hazırlama hizmetidir.
    /// </summary>
    public IBrowserSetupService BrowserSetup { get; }

    /// <summary>
    /// Uygulamanın gerçek servislerini oluşturur.
    /// </summary>
    public AppServiceProvider()
    {
        var browserSession = new PlaywrightBrowserSession();

        TTWarsConnection = new TTWarsConnectionService(browserSession);
        BrowserSetup = new PlaywrightBrowserSetupService();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        return TTWarsConnection.DisposeAsync();
    }
}
