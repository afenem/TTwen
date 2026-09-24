using Microsoft.Playwright;
using TTwen.Application.Interfaces;
using TTwen.Application.Models;

namespace TTwen.Infrastructure.Browser;

/// <summary>
/// Playwright Chromium bileşenlerini kuran Infrastructure hizmetidir.
/// </summary>
/// <remarks>
/// Kurulum işlemi uygulama başlangıcına zorunlu indirme eklemek yerine kullanıcı
/// tarafından açıkça çalıştırılabilir. Böylece ilk açılış davranışı öngörülebilir kalır.
/// </remarks>
public sealed class PlaywrightBrowserSetupService : IBrowserSetupService
{
    private static readonly string[] ChromiumInstallArguments = ["install", "chromium"];
    /// <inheritdoc />
    public async Task<BrowserSetupResult> InstallChromiumAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var exitCode = await Task.Run(
                () => Program.Main(ChromiumInstallArguments),
                cancellationToken);

            return exitCode == 0
                ? new BrowserSetupResult(
                    true,
                    "Playwright Chromium hazır.",
                    null)
                : new BrowserSetupResult(
                    false,
                    "Chromium kurulumu başarısız oldu.",
                    $"Playwright kurulum programı {exitCode} koduyla sonlandı.");
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return new BrowserSetupResult(
                false,
                "Chromium kurulumu sırasında hata oluştu.",
                exception.Message);
        }
    }
}
