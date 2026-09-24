using TTwen.Application.Interfaces;
using TTwen.Domain.Combat;
using TTwen.Domain.Entities;

namespace TTwen.Simulator.Combat;

/// <summary>
/// İlk yerel savaş simulator'üdür.
/// </summary>
/// <remarks>
/// Bu motor şimdilik mimari prototiptir. Gerçek TTWars savaş raporlarıyla
/// karşılaştırıldıktan sonra katsayıları ve rule set'i kalibre edilecektir.
/// Böylece kesin olmayan varsayımlar kodun her yerine dağılmaz.
/// </remarks>
public sealed class LocalCombatSimulator : ICombatSimulator
{
    /// <summary>
    /// Kayıp oranının saldırı/savunma oranına hassasiyetini belirler.
/// </summary>
    public double LossExponent { get; init; } = 1.5;

    public CombatSimulationResult Simulate(
        Oasis oasis,
        IReadOnlyCollection<TroopGroup> army)
    {
        var attackerPower = army.Sum(
            stack => stack.Troop.Attack * stack.Count);

        // İlk iskelette doğa savunmasını sade bir temel katsayıyla temsil ediyoruz.
        // Gerçek TTWars değeri doğrulanınca AnimalRuleSet'e taşınacaktır.
        var defenderPower = oasis.Animals.Sum(
            pair => pair.Value * 50);

        if (attackerPower <= 0)
        {
            return new CombatSimulationResult(
                false,
                0,
                defenderPower,
                1,
                new Dictionary<string, int>(),
                new Dictionary<string, int>());
        }

        var ratio = attackerPower / Math.Max(1, defenderPower);
        var success = ratio >= 1;

        var lossRatio = success
            ? Math.Min(1, Math.Pow(1 / ratio, LossExponent))
            : 1;

        var troopLosses = army.ToDictionary(
            stack => stack.Troop.Key,
            stack => Math.Min(
                stack.Count,
                (int)Math.Ceiling(stack.Count * lossRatio)));

        var animalKills = oasis.Animals.ToDictionary(
            pair => pair.Key.ToString(),
            pair => success ? pair.Value : 0);

        return new CombatSimulationResult(
            success,
            attackerPower,
            defenderPower,
            lossRatio,
            troopLosses,
            animalKills);
    }
}
