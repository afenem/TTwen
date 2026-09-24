using TTwen.Domain.Enums;
using TTwen.Domain.Snapshots;

namespace TTwen.Application.Services;

/// <summary>
/// Seçili köyün kaynak snapshot'ından kapasite ve stok durumlarını hesaplar.
/// </summary>
public static class VillageResourceStatusCalculator
{
    /// <summary>
    /// Doluluk oranı bu değerin altındaysa kaynak düşük stok olarak işaretlenir.
    /// </summary>
    public const decimal LowStockThreshold = 20m;

    /// <summary>
    /// Doluluk oranı bu değerin üstündeyse kapasite taşma riski olarak işaretlenir.
    /// </summary>
    public const decimal OverflowRiskThreshold = 90m;

    /// <summary>
    /// Köy snapshot'ındaki dört temel kaynak için durum satırlarını üretir.
    /// </summary>
    /// <param name="village">Analiz edilecek köy snapshot'ı.</param>
    /// <returns>Odun, tuğla, demir ve tahıl durumları.</returns>
    public static IReadOnlyList<VillageResourceStatus> Analyze(VillageSnapshot village)
    {
        ArgumentNullException.ThrowIfNull(village);

        return
        [
            Create(
                ResourceType.Wood,
                village.Resources.Wood,
                village.Resources.WarehouseCapacity,
                village.Production.WoodPerHour),
            Create(
                ResourceType.Clay,
                village.Resources.Clay,
                village.Resources.WarehouseCapacity,
                village.Production.ClayPerHour),
            Create(
                ResourceType.Iron,
                village.Resources.Iron,
                village.Resources.WarehouseCapacity,
                village.Production.IronPerHour),
            Create(
                ResourceType.Crop,
                village.Resources.Crop,
                village.Resources.GranaryCapacity,
                village.Production.CropPerHour)
        ];
    }

    /// <summary>
    /// Tek bir kaynak için normalize edilmiş durum kaydını oluşturur.
    /// </summary>
    private static VillageResourceStatus Create(
        ResourceType resourceType,
        int? current,
        int? capacity,
        int? perHour)
    {
        decimal? fillPercentage = null;

        if (current.HasValue && capacity is > 0)
        {
            fillPercentage = decimal.Round(
                current.Value * 100m / capacity.Value,
                1,
                MidpointRounding.AwayFromZero);
        }

        var isLowStock =
            fillPercentage.HasValue
            && fillPercentage.Value < LowStockThreshold;

        var isOverflowRisk =
            fillPercentage.HasValue
            && fillPercentage.Value >= OverflowRiskThreshold;

        return new VillageResourceStatus(
            resourceType,
            current,
            capacity,
            perHour,
            fillPercentage,
            isLowStock,
            isOverflowRisk);
    }
}