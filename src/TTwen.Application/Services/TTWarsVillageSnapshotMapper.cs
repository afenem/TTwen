using TTwen.Application.Models;
using TTwen.Domain.Snapshots;
using TTwen.Domain.ValueObjects;

namespace TTwen.Application.Services;

/// <summary>
/// Ham TTWars sayfa verisini domain seviyesindeki köy snapshot'ına dönüştürür.
/// </summary>
public static class TTWarsVillageSnapshotMapper
{
    /// <summary>
    /// Ham sayfa verisini doğrular ve domain snapshot'ına dönüştürür.
    /// </summary>
    /// <param name="data">Playwright tarafından çıkarılmış köy sayfası verisi.</param>
    /// <param name="capturedAtUtc">Verinin alındığı UTC zaman damgası.</param>
    /// <returns>Domain köy snapshot'ı.</returns>
    public static VillageSnapshot Map(
        TTWarsVillagePageData data,
        DateTimeOffset capturedAtUtc)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (string.IsNullOrWhiteSpace(data.Id))
            throw new ArgumentException("Köy kimliği boş olamaz.", nameof(data));

        if (string.IsNullOrWhiteSpace(data.Name))
            throw new ArgumentException("Köy adı boş olamaz.", nameof(data));

        Coordinates? coordinates = null;

        if (data.X.HasValue && data.Y.HasValue)
            coordinates = new Coordinates(data.X.Value, data.Y.Value);

        return new VillageSnapshot(
            Id: data.Id,
            Name: data.Name,
            Coordinates: coordinates,
            Population: data.Population,
            IsCapital: data.IsCapital,
            Resources: new VillageResources(
                data.Wood,
                data.Clay,
                data.Iron,
                data.Crop,
                data.WarehouseCapacity,
                data.GranaryCapacity),
            Production: new VillageProduction(
                data.WoodPerHour,
                data.ClayPerHour,
                data.IronPerHour,
                data.CropPerHour),
            HasResourceData: data.HasResourceData,
            HasProductionData: data.HasProductionData,
            CapturedAtUtc: capturedAtUtc,
            SourceUrl: data.SourceUrl);
    }
}