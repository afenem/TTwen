using System.Globalization;
using TTwen.Domain.Snapshots;

namespace TTwen.App.Views.Pages;

/// <summary>
/// Domain köy snapshot'ını kontrol merkezindeki tek tablo satırına dönüştürür.
/// </summary>
public sealed class VillageRow
{
    /// <summary>Satırın dayandığı canlı domain snapshot'ıdır.</summary>
    public VillageSnapshot Snapshot { get; }

    /// <summary>Satırda gösterilen köy adıdır.</summary>
    public string Name { get; }

    /// <summary>Satırda gösterilen köy kimliğidir.</summary>
    public string Id { get; }

    /// <summary>Koordinat gösterimidir.</summary>
    public string CoordinatesText { get; }

    /// <summary>Nüfus gösterimidir.</summary>
    public string PopulationText { get; }

    /// <summary>Dört kaynağın anlık stok gösterimidir.</summary>
    public string ResourcesText { get; }

    /// <summary>Depo ve ambar kapasitesinin gösterimidir.</summary>
    public string StorageText { get; }

    /// <summary>Saatlik üretim değerlerinin gösterimidir.</summary>
    public string ProductionText { get; }

    /// <summary>Snapshot'ın veri kalitesini özetleyen kısa durumdur.</summary>
    public string StatusText { get; }

    /// <summary>Domain snapshot'ından UI satırı oluşturur.</summary>
    public VillageRow(VillageSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        Snapshot = snapshot;

        Name = snapshot.IsCapital == true
            ? $"{snapshot.Name} • BAŞKENT"
            : snapshot.Name;

        Id = snapshot.Id;
        CoordinatesText = snapshot.Coordinates is { } coordinates ? coordinates.ToString() : "—";
        PopulationText = Format(snapshot.Population);

        ResourcesText =
            $"O {Format(snapshot.Resources.Wood)} · "
            + $"T {Format(snapshot.Resources.Clay)} · "
            + $"D {Format(snapshot.Resources.Iron)} · "
            + $"H {Format(snapshot.Resources.Crop)}";

        StorageText =
            $"Depo {Format(snapshot.Resources.WarehouseCapacity)} · "
            + $"Ambar {Format(snapshot.Resources.GranaryCapacity)}";

        ProductionText =
            $"O {Format(snapshot.Production.WoodPerHour)} · "
            + $"T {Format(snapshot.Production.ClayPerHour)} · "
            + $"D {Format(snapshot.Production.IronPerHour)} · "
            + $"H {Format(snapshot.Production.CropPerHour)}";

        var missing = new List<string>();
        if (!snapshot.Coordinates.HasValue) missing.Add("koordinat yok");
        if (!snapshot.HasResourceData) missing.Add("kaynak işareti yok");
        if (!snapshot.HasProductionData) missing.Add("üretim yok");

        StatusText = missing.Count == 0
            ? "CANLI OKUMA"
            : string.Join(" • ", missing);
    }

    /// <summary>Nullable sayıları Türkçe binlik ayırıcıyla gösterir.</summary>
    private static string Format(int? value)
    {
        return value?.ToString("N0", CultureInfo.GetCultureInfo("tr-TR")) ?? "—";
    }
}