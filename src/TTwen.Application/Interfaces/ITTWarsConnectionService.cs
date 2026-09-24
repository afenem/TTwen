using TTwen.Application.Models;

namespace TTwen.Application.Interfaces;

/// <summary>
/// Uygulama arayüzüne sunulan TTWars bağlantı ve canlı veri servisidir.
/// </summary>
public interface ITTWarsConnectionService : IAsyncDisposable
{
    /// <summary>Geçerli TTWars bağlantısının açık olup olmadığını gösterir.</summary>
    bool IsConnected { get; }

    /// <summary>Aktif sunucunun normalize edilmiş URL'sidir.</summary>
    string? CurrentServerUrl { get; }

    /// <summary>Aktif web sayfasının başlığıdır.</summary>
    string? CurrentPageTitle { get; }

    /// <summary>TTWars sunucusuna bağlanır.</summary>
    Task<TTWarsConnectionResult> ConnectAsync(
        string serverUrl,
        CancellationToken cancellationToken);

    /// <summary>Canlı köy snapshot'larını okur.</summary>
    Task<TTWarsVillageReadResult> ReadVillagesAsync(
        CancellationToken cancellationToken);

    /// <summary>Seçilen köyün bina snapshot'larını okur.</summary>
    Task<TTWarsBuildingReadResult> ReadBuildingsAsync(
        string? villageId,
        CancellationToken cancellationToken);

    /// <summary>Açık TTWars tarayıcı oturumunu kapatır.</summary>
    Task DisconnectAsync();
}