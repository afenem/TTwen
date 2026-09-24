using TTwen.Domain.Combat;
using TTwen.Domain.Entities;

namespace TTwen.Application.Interfaces;

/// <summary>
/// Savaş motorunun application katmanına sunduğu sözleşmedir.
/// </summary>
/// <remarks>
/// Bu interface sayesinde UI veya TTWars adapter'ı belirli bir savaş formülüne
/// bağlanmaz. Gerçek ve test simulator'ları aynı sözleşmeyi uygulayabilir.
/// </remarks>
public interface ICombatSimulator
{
    CombatSimulationResult Simulate(
        Oasis oasis,
        IReadOnlyCollection<TroopGroup> army);
}
