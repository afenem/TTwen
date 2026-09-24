using TTwen.Domain.Entities;

namespace TTwen.Application.Interfaces;

/// <summary>
/// TTWars işlemlerinin uygulama katmanına sunduğu soyut sözleşmedir.
/// </summary>
/// <remarks>
/// UI, Playwright selector'ı veya HTML ayrıntısı bilmez. Örneğin UI "vahaları
/// tara" der; bunun nasıl gerçekleştirileceğini Infrastructure belirler.
/// </remarks>
public interface ITTWarsClient
{
    Task ConnectAsync(string serverUrl, CancellationToken cancellationToken);

    Task<IReadOnlyList<Village>> ReadVillagesAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Oasis>> ScanOasesAsync(
        CancellationToken cancellationToken);
}
