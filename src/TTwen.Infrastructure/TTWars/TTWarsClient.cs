using Microsoft.Playwright;
using TTwen.Application.Interfaces;
using TTwen.Application.Models;
using TTwen.Domain.Entities;
using TTwen.Domain.ValueObjects;

namespace TTwen.Infrastructure.TTWars;

/// <summary>
/// TTWars web arayüzüne bağlanan Infrastructure adapter'ıdır.
/// </summary>
public sealed class TTWarsClient : ITTWarsClient
{
    private readonly IPage _page;
    private readonly TTWarsVillageReader _villageReader;

    /// <summary>
    /// Belirli bir Playwright sayfası üzerinden TTWars istemcisi oluşturur.
    /// </summary>
    public TTWarsClient(IPage page)
    {
        _page = page ?? throw new ArgumentNullException(nameof(page));
        _villageReader = new TTWarsVillageReader(_page);
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
    public Task<TTWarsVillageReadResult> ReadVillagesAsync(
        CancellationToken cancellationToken)
    {
        return _villageReader.ReadAllAsync(cancellationToken);
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