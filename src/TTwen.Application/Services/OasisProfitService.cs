using TTwen.Application.Interfaces;
using TTwen.Domain.Combat;
using TTwen.Domain.Entities;

namespace TTwen.Application.Services;

/// <summary>
/// Savaş sonucunu kaynak ve kâr sonucuna çevirir.
/// </summary>
/// <remarks>
/// Savaş ile ekonomi ayrı sorumluluklardır. Simulator askeri sonucu üretir,
/// bu servis o sonucu ekonomik ölçülere dönüştürür.
/// </remarks>
public sealed class OasisProfitService
{
    private const int ResourcePerLootUnit = 40;

    private readonly ICombatSimulator _combatSimulator;

    public OasisProfitService(ICombatSimulator combatSimulator)
    {
        _combatSimulator = combatSimulator;
    }

    /// <summary>
    /// Bir ordunun vaha yağmasından elde edeceği ekonomik sonucu hesaplar.
    /// </summary>
    public OasisProfitResult Evaluate(
        Oasis oasis,
        IReadOnlyCollection<TroopGroup> army,
        double roundTripSeconds)
    {
        var combat = _combatSimulator.Simulate(oasis, army);

        var lootUnits = combat.AnimalKills.Values.Sum();
        var wood = lootUnits * ResourcePerLootUnit;
        var clay = lootUnits * ResourcePerLootUnit;
        var iron = lootUnits * ResourcePerLootUnit;
        var crop = lootUnits * ResourcePerLootUnit;

        var lostTroopCost = 0;
        var gross = wood + clay + iron + crop;
        var net = gross - lostTroopCost;

        var resourcesPerHour = roundTripSeconds > 0
            ? net / (roundTripSeconds / 3600.0)
            : 0;

        return new OasisProfitResult(
            combat,
            wood,
            clay,
            iron,
            crop,
            lostTroopCost,
            net,
            resourcesPerHour);
    }
}
