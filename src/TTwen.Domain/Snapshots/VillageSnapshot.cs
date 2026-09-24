using TTwen.Domain.ValueObjects;

namespace TTwen.Domain.Snapshots;

/// <summary>
/// TTWars web sayfasından tek bir okuma anında elde edilen köy durumunu temsil eder.
/// </summary>
/// <remarks>
/// Snapshot geçici gözlemdir. Kalıcı köy ayarları ve otomasyon profilleri ile
/// karıştırılmaz; canlı web verisi ile kullanıcı konfigürasyonu ayrı tutulur.
/// </remarks>
public sealed record VillageSnapshot(
    string Id,
    string Name,
    Coordinates? Coordinates,
    int? Population,
    bool? IsCapital,
    VillageResources Resources,
    VillageProduction Production,
    bool HasResourceData,
    bool HasProductionData,
    DateTimeOffset CapturedAtUtc,
    string SourceUrl);