namespace TTwen.Domain.Enums;

/// <summary>
/// Oyundaki dört temel hammadde türünü temsil eder.
/// </summary>
/// <remarks>
/// Enum kullanımı, farklı modüllerde "wood", "Wood", "odun" gibi dağınık
/// string değerleri kullanılmasını önler.
/// </remarks>
public enum ResourceType
{
    Wood,
    Clay,
    Iron,
    Crop
}
