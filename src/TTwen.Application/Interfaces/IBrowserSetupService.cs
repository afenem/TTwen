using TTwen.Application.Models;

namespace TTwen.Application.Interfaces;

/// <summary>
/// Playwright'ın çalışması için gerekli tarayıcı bileşenlerini hazırlayan sözleşmedir.
/// </summary>
public interface IBrowserSetupService
{
    /// <summary>
    /// Playwright Chromium tarayıcısını kurar veya günceller.
    /// </summary>
    Task<BrowserSetupResult> InstallChromiumAsync(
        CancellationToken cancellationToken);
}
