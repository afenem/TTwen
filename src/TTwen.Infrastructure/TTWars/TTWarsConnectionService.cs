using Microsoft.Playwright;
using TTwen.Application.Interfaces;
using TTwen.Application.Models;
using TTwen.Infrastructure.Browser;

namespace TTwen.Infrastructure.TTWars;

/// <summary>
/// TTWars bağlantısının gerçek Playwright uygulamasıdır.
/// </summary>
public sealed class TTWarsConnectionService : ITTWarsConnectionService
{
    private readonly PlaywrightBrowserSession _browserSession;
    private TTWarsClient? _client;

    /// <summary>Yeni TTWars bağlantı hizmeti oluşturur.</summary>
    public TTWarsConnectionService(PlaywrightBrowserSession browserSession)
    {
        _browserSession = browserSession;
    }

    /// <inheritdoc />
    public bool IsConnected { get; private set; }

    /// <inheritdoc />
    public string? CurrentServerUrl { get; private set; }

    /// <inheritdoc />
    public string? CurrentPageTitle { get; private set; }

    /// <inheritdoc />
    public async Task<TTWarsConnectionResult> ConnectAsync(
        string serverUrl,
        CancellationToken cancellationToken)
    {
        try
        {
            var address = TTwen.Domain.ValueObjects.TTWarsServerAddress.Parse(serverUrl);
            var profileDirectory = GetBrowserProfileDirectory();

            await _browserSession.StartAsync(profileDirectory);

            var page = _browserSession.CurrentPage
                ?? throw new InvalidOperationException("Playwright sayfası oluşturulamadı.");

            _client = new TTWarsClient(page);
            await _client.ConnectAsync(address.Value, cancellationToken);

            CurrentServerUrl = address.Value;
            CurrentPageTitle = await page.TitleAsync();
            IsConnected = true;

            return new TTWarsConnectionResult(
                true,
                "TTWars sunucusuna bağlantı başarılı.",
                CurrentServerUrl,
                CurrentPageTitle,
                null);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (PlaywrightException exception)
        {
            IsConnected = false;
            return new TTWarsConnectionResult(false, "Playwright bağlantısı başarısız oldu.", CurrentServerUrl, CurrentPageTitle, exception.Message);
        }
        catch (ArgumentException exception)
        {
            IsConnected = false;
            return new TTWarsConnectionResult(false, "Sunucu adresi geçersiz.", null, null, exception.Message);
        }
        catch (Exception exception)
        {
            IsConnected = false;
            return new TTWarsConnectionResult(false, "TTWars bağlantısı sırasında beklenmeyen bir hata oluştu.", CurrentServerUrl, CurrentPageTitle, exception.Message);
        }
    }

    /// <inheritdoc />
    public async Task<TTWarsVillageReadResult> ReadVillagesAsync(
        CancellationToken cancellationToken)
    {
        if (!IsConnected || _client is null)
        {
            return new TTWarsVillageReadResult(
                false,
                Array.Empty<TTwen.Domain.Snapshots.VillageSnapshot>(),
                0,
                0,
                "TTWars bağlantısı açık değil.",
                "Önce TTWars Sunucu ekranından bağlantı kurun.",
                DateTimeOffset.UtcNow);
        }

        try
        {
            return await _client.ReadVillagesAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return new TTWarsVillageReadResult(
                false,
                Array.Empty<TTwen.Domain.Snapshots.VillageSnapshot>(),
                0,
                0,
                "Köy bilgileri okunamadı.",
                exception.Message,
                DateTimeOffset.UtcNow);
        }
    }

    /// <inheritdoc />
    public async Task<TTWarsVillageReadResult> ReadVillageAsync(
        string villageId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(villageId))
            throw new ArgumentException("Köy kimliği boş olamaz.", nameof(villageId));

        if (!IsConnected || _client is null)
        {
            return new TTWarsVillageReadResult(
                false,
                Array.Empty<TTwen.Domain.Snapshots.VillageSnapshot>(),
                1,
                1,
                "TTWars bağlantısı açık değil.",
                "Önce TTWars Sunucu ekranından bağlantı kurun.",
                DateTimeOffset.UtcNow);
        }

        try
        {
            return await _client.ReadVillageAsync(villageId, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return new TTWarsVillageReadResult(
                false,
                Array.Empty<TTwen.Domain.Snapshots.VillageSnapshot>(),
                1,
                1,
                "Seçili köy bilgileri okunamadı.",
                exception.Message,
                DateTimeOffset.UtcNow);
        }
    }

    /// <inheritdoc />
    public async Task<TTWarsBuildingReadResult> ReadBuildingsAsync(
        string? villageId,
        CancellationToken cancellationToken)
    {
        if (!IsConnected || _client is null)
        {
            return new TTWarsBuildingReadResult(
                false,
                Array.Empty<TTwen.Domain.Snapshots.BuildingSnapshot>(),
                "TTWars bağlantısı açık değil.",
                "Önce TTWars Sunucu ekranından bağlantı kurun.",
                DateTimeOffset.UtcNow,
                CurrentServerUrl ?? string.Empty);
        }

        try
        {
            return await _client.ReadBuildingsAsync(villageId, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return new TTWarsBuildingReadResult(
                false,
                Array.Empty<TTwen.Domain.Snapshots.BuildingSnapshot>(),
                "Bina bilgileri okunamadı.",
                exception.Message,
                DateTimeOffset.UtcNow,
                CurrentServerUrl ?? string.Empty);
        }
    }

    /// <inheritdoc />
    public async Task DisconnectAsync()
    {
        await _browserSession.DisposeAsync();
        _client = null;
        IsConnected = false;
        CurrentServerUrl = null;
        CurrentPageTitle = null;
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync() => new(DisconnectAsync());

    /// <summary>TTwen için yalnızca kendi Chromium profilinin tutulacağı klasörü üretir.</summary>
    private static string GetBrowserProfileDirectory()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        if (string.IsNullOrWhiteSpace(localAppData))
            throw new InvalidOperationException("Windows LocalAppData klasörü bulunamadı.");

        var profileDirectory = Path.Combine(localAppData, "TTwen", "Playwright", "Profile");
        Directory.CreateDirectory(profileDirectory);
        return profileDirectory;
    }
}