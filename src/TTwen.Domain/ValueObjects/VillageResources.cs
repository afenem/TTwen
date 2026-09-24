namespace TTwen.Domain.ValueObjects;

/// <summary>
/// Bir köydeki anlık dört temel kaynak stoğunu ve depolama kapasitelerini temsil eder.
/// </summary>
/// <remarks>
/// Değerlerin nullable tutulması, web sayfasındaki bir alanın bulunamaması durumunda
/// bilinmeyen değer ile gerçek sıfır değerinin birbirine karıştırılmasını önler.
/// </remarks>
public readonly record struct VillageResources(
    int? Wood,
    int? Clay,
    int? Iron,
    int? Crop,
    int? WarehouseCapacity,
    int? GranaryCapacity);