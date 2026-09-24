using System.Globalization;
using TTwen.Application.Models;
using TTwen.Application.Services;
using Xunit;

namespace TTwen.Tests;

/// <summary>
/// TTWars ham köy verisinin domain snapshot'ına dönüşümünü doğrular.
/// </summary>
public sealed class TTWarsVillageSnapshotMapperTests
{
    /// <summary>
    /// Normal bir sayfa verisinin temel alanlarını koruduğunu doğrular.
    /// </summary>
    [Fact]
    public void MapsPageDataToVillageSnapshot()
    {
        var capturedAt = DateTimeOffset.Parse("2026-09-24T09:00:00Z", CultureInfo.InvariantCulture);

        var data = new TTWarsVillagePageData
        {
            Id = "123",
            Name = "Başkent",
            X = -12,
            Y = 34,
            Population = 456,
            IsCapital = true,
            Wood = 1000,
            Clay = 1100,
            Iron = 1200,
            Crop = 1300,
            WarehouseCapacity = 1600,
            GranaryCapacity = 1800,
            WoodPerHour = 250,
            ClayPerHour = 260,
            IronPerHour = 270,
            CropPerHour = 280,
            HasResourceData = true,
            HasProductionData = true,
            SourceUrl = "https://example.invalid/dorf1.php"
        };

        var snapshot = TTWarsVillageSnapshotMapper.Map(data, capturedAt);

        Assert.Equal("123", snapshot.Id);
        Assert.Equal("Başkent", snapshot.Name);
        Assert.Equal(-12, snapshot.Coordinates?.X);
        Assert.Equal(34, snapshot.Coordinates?.Y);
        Assert.Equal(456, snapshot.Population);
        Assert.True(snapshot.IsCapital);
        Assert.Equal(1000, snapshot.Resources.Wood);
        Assert.Equal(1800, snapshot.Resources.GranaryCapacity);
        Assert.Equal(280, snapshot.Production.CropPerHour);
        Assert.Equal(capturedAt, snapshot.CapturedAtUtc);
    }

    /// <summary>
    /// Eksik sayfa alanlarının bilinmeyen olarak korunabildiğini doğrular.
    /// </summary>
    [Fact]
    public void PreservesUnknownPageFieldsAsUnknown()
    {
        var data = new TTWarsVillagePageData
        {
            Id = "7",
            Name = "Köy",
            SourceUrl = "https://example.invalid/dorf1.php"
        };

        var snapshot = TTWarsVillageSnapshotMapper.Map(
            data,
            DateTimeOffset.UtcNow);

        Assert.Null(snapshot.Coordinates);
        Assert.Null(snapshot.Population);
        Assert.Null(snapshot.Resources.Wood);
        Assert.Null(snapshot.Resources.WarehouseCapacity);
        Assert.Null(snapshot.Production.WoodPerHour);
        Assert.False(snapshot.HasResourceData);
        Assert.False(snapshot.HasProductionData);
    }
}