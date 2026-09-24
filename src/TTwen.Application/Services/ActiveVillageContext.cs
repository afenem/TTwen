using TTwen.Application.Interfaces;
using TTwen.Domain.Snapshots;

namespace TTwen.Application.Services;

/// <summary>
/// Uygulama çalışma süresince seçili köyü tutan hafif durum servisidir.
/// </summary>
/// <remarks>
/// Kalıcı ayar değildir. UI modüllerinin aynı köy üzerinde tutarlı çalışmasını sağlamak
/// için Process scope içinde yaşar; daha sonra kalıcı seçim gerekirse ayrı bir settings
/// katmanına taşınabilir.
/// </remarks>
public sealed class ActiveVillageContext : IActiveVillageContext
{
    /// <inheritdoc />
    public VillageSnapshot? CurrentVillage { get; private set; }

    /// <inheritdoc />
    public event EventHandler? Changed;

    /// <inheritdoc />
    public void SetCurrent(VillageSnapshot village)
    {
        ArgumentNullException.ThrowIfNull(village);

        if (string.Equals(
                CurrentVillage?.Id,
                village.Id,
                StringComparison.Ordinal))
        {
            CurrentVillage = village;
            Changed?.Invoke(this, EventArgs.Empty);
            return;
        }

        CurrentVillage = village;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void Clear()
    {
        if (CurrentVillage is null)
            return;

        CurrentVillage = null;
        Changed?.Invoke(this, EventArgs.Empty);
    }
}