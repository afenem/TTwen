using TTwen.Domain.Snapshots;

namespace TTwen.Application.Interfaces;

/// <summary>
/// Uygulama genelinde seçili köyü paylaşan durum sözleşmesidir.
/// </summary>
public interface IActiveVillageContext
{
    /// <summary>Şu anda seçili olan köy snapshot'ıdır.</summary>
    VillageSnapshot? CurrentVillage { get; }

    /// <summary>Seçili köy değiştiğinde tetiklenir.</summary>
    event EventHandler? Changed;

    /// <summary>Verilen köyü aktif seçim olarak belirler.</summary>
    void SetCurrent(VillageSnapshot village);

    /// <summary>Aktif köy seçimini temizler.</summary>
    void Clear();
}