using TTwen.Domain.Entities;

namespace TTwen.Domain.Combat;

/// <summary>
/// Saldırıya katılan bir asker tipini ve miktarını tutar.
/// </summary>
public sealed record TroopStack(Troop Troop, int Count);

/// <summary>
/// Bir savaş simülasyonunun temel sonucudur.
/// </summary>
public sealed record CombatSimulationResult(
    bool Success,
    double AttackerPower,
    double DefenderPower,
    double AttackerLossRatio,
    IReadOnlyDictionary<string, int> TroopLosses,
    IReadOnlyDictionary<string, int> AnimalKills);

/// <summary>
/// Vaha savaş sonucunun ekonomik karşılığını temsil eder.
/// </summary>
public sealed record OasisProfitResult(
    CombatSimulationResult Combat,
    int Wood,
    int Clay,
    int Iron,
    int Crop,
    int LostTroopCost,
    int NetProfit,
    double ResourcesPerHour)
{
    public int GrossResources => Wood + Clay + Iron + Crop;

    public bool IsProfitable => Combat.Success && NetProfit > 0;
}
