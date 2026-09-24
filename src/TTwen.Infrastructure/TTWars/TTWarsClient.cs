using Microsoft.Playwright;
using TTwen.Application.Interfaces;
using TTwen.Domain.Entities;

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

    public TTWarsClient(IPage page)
    {
        _page = page;
    }

    public async Task ConnectAsync(
        string serverUrl,
        CancellationToken cancellationToken)
    {
        var url = NormalizeServerUrl(serverUrl);

        await _page.GotoAsync(
            url,
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });
    }

    public Task<IReadOnlyList<Village>> ReadVillagesAsync(
        CancellationToken cancellationToken)
    {
        // Gerçek HTML doğrulandıktan sonra VillageReader burada çağrılacaktır.
        IReadOnlyList<Village> result = Array.Empty<Village>();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<Oasis>> ScanOasesAsync(
        CancellationToken cancellationToken)
    {
        // Gerçek harita yapısı doğrulandıktan sonra OasisReader burada çağrılacaktır.
        IReadOnlyList<Oasis> result = Array.Empty<Oasis>();
        return Task.FromResult(result);
    }

    private static string NormalizeServerUrl(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException(
                "TTWars sunucu adresi boş olamaz.",
                nameof(input));

        var value = input.Trim();

        if (!value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            && !value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            value = "https://" + value;
        }

        return value.TrimEnd('/');
    }
}
