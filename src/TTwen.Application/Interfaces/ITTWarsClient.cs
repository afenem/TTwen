using TTwen.Application.Models;

namespace TTwen.Application.Interfaces;

/// <summary>
/// TTWars işlemlerinin uygulama katmanına sunduğu web adapter sözleşmesidir.
/// </summary>
public interface ITTWarsClient
{
    /// <summary>Verilen TTWars adresine gider.</summary>
    Task ConnectAsync(string serverUrl, CancellationToken cancellationToken);

    /// <summary>Hesabın erişebildiği köyleri canlı web sayfalarından okur.</summary>
    Task<TTWarsVillageReadResult> ReadVillagesAsync(
        CancellationToken cancellationToken);

    /// <summary>Aktif veya seçilen köyün bina slotlarını canlı dorf2.php sayfasından okur.</summary>
    Task<TTWarsBuildingReadResult> ReadBuildingsAsync(
        string? villageId,
        CancellationToken cancellationToken);

    /// <summary>Haritadaki vahaları tarar.</summary>
    Task<IReadOnlyList<TTwen.Domain.Entities.Oasis>> ScanOasesAsync(
        CancellationToken cancellationToken);
}