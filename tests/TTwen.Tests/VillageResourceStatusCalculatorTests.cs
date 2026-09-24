using TTwen.Application.Services;
using TTwen.Domain.Enums;
using TTwen.Domain.Snapshots;
using TTwen.Domain.ValueObjects;
using Xunit;

namespace TTwen.Tests;

/// <summary>
/// Köy kaynak durumu hesaplamalarının eşik ve kapasite davranışını doğrular.
/// </summary>
public sealed class VillageResourceStatusCalculatorTests
{
    /// <summary>Normal dolulukta kaynağın normal durum verdiğini doğrular.</summary>
    [Fact]
    public void CalculatesNormalResourceStatus()
    {
        var village = CreateVillage(
            wood: 500,
            clay: 500,
            iron: 500,
            crop: 500,
            warehouse: 1000,
            granary: 1000);

        var wood = VillageResourceStatusCalculator
            .Analyze(village)
            .Single(x => x.ResourceType == ResourceType.Wood);

        Assert.Equal(50m, wood.FillPercentage);
        Assert.False(wood.IsLowStock);
        Assert.False(wood.IsOverflowRisk);
    }

    /// <summary>Düşük stok eşiğinin altındaki değeri işaretlediğini doğrular.</summary>
    [Fact]
    public void FlagsLowStockBelowTwentyPercent()
    {
        var village = CreateVillage(
            wood: 199,
            clay: 500,
            iron: 500,
            crop: 500,
            warehouse: 1000,
            granary: 1000);

        var wood = VillageResourceStatusCalculator
            .Analyze(village)
            .Single(x => x.ResourceType == ResourceType.Wood);

        Assert.Equal(19.9m, wood.FillPercentage);
        Assert.True(wood.IsLowStock);
        Assert.False(wood.IsOverflowRisk);
    }

    /// <summary>Yüzde 90 ve üzerindeki değeri taşma riski olarak işaretlediğini doğrular.</summary>
    [Fact]
    public void FlagsOverflowRiskAtNinetyPercent()
    {
        var village = CreateVillage(
            wood: 900,
            clay: 500,
            iron: 500,
            crop: 500,
            warehouse: 1000,
            granary: 1000);

        var wood = VillageResourceStatusCalculator
            .Analyze(village)
            .Single(x => x.ResourceType == ResourceType.Wood);

        Assert.Equal(90m, wood.FillPercentage);
        Assert.True(wood.IsOverflowRisk);
        Assert.False(wood.IsLowStock);
    }

    /// <summary>Kapasite bilinmiyorsa doluluk yüzdesinin bilinmeyen kaldığını doğrular.</summary>
    [Fact]
    public void KeepsFillUnknownWithoutCapacity()
    {
        var village = CreateVillage(
            wood: 500,
            clay: 500,
            iron: 500,
            crop: 500,
            warehouse: null,
            granary: null);

        var wood = VillageResourceStatusCalculator
            .Analyze(village)
            .Single(x => x.ResourceType == ResourceType.Wood);

        Assert.Null(wood.FillPercentage);
        Assert.False(wood.IsLowStock);
        Assert.False(wood.IsOverflowRisk);
    }

    private static VillageSnapshot CreateVillage(
        int? wood,
        int? clay,
        int? iron,
        int? crop,
        int? warehouse,
        int? granary)
    {
        return new VillageSnapshot(
            "1",
            "Test Köyü",
            null,
            100,
            false,
            new VillageResources(
                wood,
                clay,
                iron,
                crop,
                warehouse,
                granary),
            new VillageProduction(
                100,
                100,
                100,
                100),
            true,
            true,
            DateTimeOffset.UtcNow,
            "https://example.invalid/dorf1.php");
    }
}