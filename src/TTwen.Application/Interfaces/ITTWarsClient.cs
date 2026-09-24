using TTwen.Application.Models;

namespace TTwen.Application.Interfaces;

/// <summary>
/// TTWars işlemlerinin uygulama katmanına sunduğu web adapter sözleşmesidir.
/// </summary>
/// <remarks>
/// UI, Playwright selector'ı veya HTML ayrıntısı bilmez. Web'e özgü okuma ve
/// navigasyon işlemleri Infrastructure katmanında kalır.
/// </remarks>
public interface ITTWarsClient
{
    /// <summary>Verilen TTWars adresine gider.</summary>
    Task ConnectAsync(string serverUrl, CancellationToken cancellationToken);

    /// <summary>Hesabın erişebildiği köyleri canlı web sayfalarından okur.</summary>
    Task<TTWarsVillageReadResult> ReadVillagesAsync(
        CancellationToken cancellationToken);

    /// <summary>Haritadaki vahaları tarar.</summary>
    Task<IReadOnlyList<TTwen.Domain.Entities.Oasis>> ScanOasesAsync(
        CancellationToken cancellationToken);
}