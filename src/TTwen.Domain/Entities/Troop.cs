using TTwen.Domain.Enums;

namespace TTwen.Domain.Entities;

/// <summary>
/// Bir asker tipinin savaş ve ekonomik özelliklerini temsil eder.
/// </summary>
/// <remarks>
/// Asker bilgisi domain içinde tutulduğu için simulator ve UI aynı veri modelini
/// kullanabilir; hiçbirinin web sayfasından doğrudan asker bilgisi okuması gerekmez.
/// </remarks>
public sealed class Troop
{
    public string Key { get; init; } = "";
    public string Name { get; init; } = "";
    public UnitCategory Category { get; init; }
    public int Attack { get; init; }
    public int DefenseAgainstInfantry { get; init; }
    public int DefenseAgainstCavalry { get; init; }
    public int CropConsumption { get; init; }
    public int CostValue { get; init; }
}
