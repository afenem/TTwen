using TTwen.Domain.ValueObjects;

namespace TTwen.Domain.Entities;

/// <summary>
/// Tek bir oyuncu köyünü temsil eder.
/// </summary>
public sealed class Village
{
    public string Id { get; init; } = "";
    public string Name { get; set; } = "";
    public Coordinates Coordinates { get; init; }
    public int Population { get; set; }

    /// <summary>
    /// Köyün otomatik yönetimde olup olmadığını belirtir.
/// </summary>
/// <remarks>
/// Bu ayar köy seviyesinde tutulur; böylece aynı hesapta bazı köyler otomatik,
/// bazıları manuel çalıştırılabilir.
/// </remarks>
    public bool AutomationEnabled { get; set; }
}
