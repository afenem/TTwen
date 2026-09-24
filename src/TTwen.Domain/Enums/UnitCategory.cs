namespace TTwen.Domain.Enums;

/// <summary>
/// Askerin piyade veya süvari olduğunu belirtir.
/// </summary>
/// <remarks>
/// Ayrı enum olması, savaş motorunun savunma türlerini doğru sınıflandırmasını
/// ve asker modelinden bağımsız şekilde aynı kuralı kullanmasını sağlar.
/// </remarks>
public enum UnitCategory
{
    Infantry,
    Cavalry
}
