using TTwen.Domain.Enums;

namespace TTwen.Domain.Snapshots;

/// <summary>
/// Tek bir temel kaynağın anlık stok, kapasite ve üretim durumunu temsil eder.
/// </summary>
/// <remarks>
/// Doluluk yüzdesi yalnızca hem mevcut stok hem de kapasite biliniyorsa hesaplanır.
/// Bu nedenle eksik HTML verisi gerçek sıfır değeriyle karıştırılmaz.
/// </remarks>
public sealed record VillageResourceStatus(
    ResourceType ResourceType,
    int? Current,
    int? Capacity,
    int? PerHour,
    decimal? FillPercentage,
    bool IsLowStock,
    bool IsOverflowRisk);