namespace TTwen.Application.Models;

/// <summary>
/// Playwright tarafından TTWars köy sayfasından çıkarılan normalize edilmiş alanları taşır.
/// </summary>
/// <remarks>
/// Bu model HTML selector ayrıntılarını Application katmanına taşımadan veri sınırı sağlar.
/// </remarks>
public sealed class TTWarsVillagePageData
{
    /// <summary>Okunan köyün bilinen sunucu kimliğidir.</summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>Sayfada görünen köy adıdır.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Köyün x koordinatıdır.</summary>
    public int? X { get; init; }

    /// <summary>Köyün y koordinatıdır.</summary>
    public int? Y { get; init; }

    /// <summary>Sayfada okunabilen köy nüfusudur.</summary>
    public int? Population { get; init; }

    /// <summary>Köyün başkent olup olduğuna dair sayfa işaretidir.</summary>
    public bool? IsCapital { get; init; }

    /// <summary>Anlık odun stokudur.</summary>
    public int? Wood { get; init; }

    /// <summary>Anlık tuğla stokudur.</summary>
    public int? Clay { get; init; }

    /// <summary>Anlık demir stokudur.</summary>
    public int? Iron { get; init; }

    /// <summary>Anlık tahıl stokudur.</summary>
    public int? Crop { get; init; }

    /// <summary>Odun, tuğla ve demir için ortak depo kapasitesidir.</summary>
    public int? WarehouseCapacity { get; init; }

    /// <summary>Tahıl ambarı kapasitesidir.</summary>
    public int? GranaryCapacity { get; init; }

    /// <summary>Saatlik odun üretimidir.</summary>
    public int? WoodPerHour { get; init; }

    /// <summary>Saatlik tuğla üretimidir.</summary>
    public int? ClayPerHour { get; init; }

    /// <summary>Saatlik demir üretimidir.</summary>
    public int? IronPerHour { get; init; }

    /// <summary>Saatlik tahıl üretimidir.</summary>
    public int? CropPerHour { get; init; }

    /// <summary>Kaynak stoklarının sayfada tanındığını belirtir.</summary>
    public bool HasResourceData { get; init; }

    /// <summary>Üretim tablosunun sayfada tanındığını belirtir.</summary>
    public bool HasProductionData { get; init; }

    /// <summary>Okumanın yapıldığı sayfanın tam URL'sidir.</summary>
    public string SourceUrl { get; init; } = string.Empty;

    /// <summary>Okuma sırasında oluşan kısa selector tanılamasıdır.</summary>
    public string? Diagnostics { get; init; }
}