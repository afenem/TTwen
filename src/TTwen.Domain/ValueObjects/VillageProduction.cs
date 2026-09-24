namespace TTwen.Domain.ValueObjects;

/// <summary>
/// Bir köyün saatlik kaynak üretim değerlerini temsil eder.
/// </summary>
public readonly record struct VillageProduction(
    int? WoodPerHour,
    int? ClayPerHour,
    int? IronPerHour,
    int? CropPerHour);