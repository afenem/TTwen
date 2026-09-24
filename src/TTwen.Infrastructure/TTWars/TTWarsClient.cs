using Microsoft.Playwright;
using TTwen.Application.Interfaces;
using TTwen.Domain.Entities;
using TTwen.Domain.ValueObjects;

namespace TTwen.Infrastructure.TTWars;

/// <summary>
/// TTWars web arayüzüne bağlanan Infrastructure adapter'ıdır.
/// </summary>
/// <remarks>
/// TTWars'a özel URL, HTML, selector ve reader kodu yalnızca bu katmanda bulunur.
/// Bu izolasyon, sunucu HTML'i değiştiğinde diğer katmanları korumak içindir.
/// </remarks>
public sealed class TTWarsClient : ITTWarsClient
{
    private readonly IPage _page;

    /// <summary>
    /// Belirli bir Playwright sayfası üzerinden TTWars istemcisi oluşturur.
    /// </summary>
    public TTWarsClient(IPage page)
    {
        _page = page ?? throw new ArgumentNullException(nameof(page));
    }

    /// <inheritdoc />
    public async Task ConnectAsync(
        string serverUrl,
        CancellationToken cancellationToken)
    {
        var address = TTWarsServerAddress.Parse(serverUrl);

        cancellationToken.ThrowIfCancellationRequested();

        await _page.GotoAsync(
            address.Value,
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });

        cancellationToken.ThrowIfCancellationRequested();
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Village>> ReadVillagesAsync(
        CancellationToken cancellationToken)
    {
        // Gerçek HTML doğrulandıktan sonra VillageReader burada çağrılacaktır.
        IReadOnlyList<Village> result = Array.Empty<Village>();
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Oasis>> ScanOasesAsync(
        CancellationToken cancellationToken)
    {
        // Gerçek harita yapısı doğrulandıktan sonra OasisReader burada çağrılacaktır.
        IReadOnlyList<Oasis> result = Array.Empty<Oasis>();
        return Task.FromResult(result);
    }
}