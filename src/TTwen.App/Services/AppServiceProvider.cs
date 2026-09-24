using TTwen.Application.Interfaces;
using TTwen.Application.Services;
using TTwen.Infrastructure.Browser;
using TTwen.Infrastructure.TTWars;

namespace TTwen.App.Services;

/// <summary>
/// WinUI uygulamasının composition root'udur.
/// </summary>
public sealed class AppServiceProvider : IAsyncDisposable
{
    /// <summary>TTWars bağlantı hizmetidir.</summary>
    public ITTWarsConnectionService TTWarsConnection { get; }

    /// <summary>Playwright tarayıcı hazırlama hizmetidir.</summary>
    public IBrowserSetupService BrowserSetup { get; }

    /// <summary>Uygulama genelinde seçili köyü paylaşan durum servisidir.</summary>
    public IActiveVillageContext ActiveVillage { get; }

    /// <summary>Uygulamanın gerçek servislerini oluşturur.</summary>
    public AppServiceProvider()
    {
        var browserSession = new PlaywrightBrowserSession();

        TTWarsConnection = new TTWarsConnectionService(browserSession);
        BrowserSetup = new PlaywrightBrowserSetupService();
        ActiveVillage = new ActiveVillageContext();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        return TTWarsConnection.DisposeAsync();
    }
}