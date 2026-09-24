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

    /// <summary>
    /// Yeni TTWars bağlantı hizmeti oluşturur.
    /// </summary>
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
                ?? throw new InvalidOperationException(
                    "Playwright sayfası oluşturulamadı.");

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

            return new TTWarsConnectionResult(
                false,
                "Playwright bağlantısı başarısız oldu.",
                CurrentServerUrl,
                CurrentPageTitle,
                exception.Message);
        }
        catch (ArgumentException exception)
        {
            IsConnected = false;

            return new TTWarsConnectionResult(
                false,
                "Sunucu adresi geçersiz.",
                null,
                null,
                exception.Message);
        }
        catch (Exception exception)
        {
            IsConnected = false;

            return new TTWarsConnectionResult(
                false,
                "TTWars bağlantısı sırasında beklenmeyen bir hata oluştu.",
                CurrentServerUrl,
                CurrentPageTitle,
                exception.Message);
        }
    }

    /// <inheritdoc />
    public async Task<TTWarsVillageReadResult> ReadVillagesAsync(
        CancellationToken cancellationToken)
    {
        var capturedAtUtc = DateTimeOffset.UtcNow;

        if (!IsConnected || _client is null)
        {
            return new TTWarsVillageReadResult(
                Success: false,
                Villages: Array.Empty<TTwen.Domain.Snapshots.VillageSnapshot>(),
                RequestedCount: 0,
                FailedCount: 0,
                Message: "TTWars bağlantısı açık değil.",
                Details: "Önce TTWars Sunucu ekranından bağlantı kurun.",
                CapturedAtUtc: capturedAtUtc);
        }

        try
        {
            return await _client.ReadVillagesAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (PlaywrightException exception)
        {
            return new TTWarsVillageReadResult(
                Success: false,
                Villages: Array.Empty<TTwen.Domain.Snapshots.VillageSnapshot>(),
                RequestedCount: 0,
                FailedCount: 0,
                Message: "Köy bilgileri Playwright üzerinden okunamadı.",
                Details: exception.Message,
                CapturedAtUtc: capturedAtUtc);
        }
        catch (Exception exception)
        {
            return new TTWarsVillageReadResult(
                Success: false,
                Villages: Array.Empty<TTwen.Domain.Snapshots.VillageSnapshot>(),
                RequestedCount: 0,
                FailedCount: 0,
                Message: "Köy bilgileri okunurken beklenmeyen bir hata oluştu.",
                Details: exception.Message,
                CapturedAtUtc: capturedAtUtc);
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
    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }

    /// <summary>
    /// TTwen için yalnızca kendi Chromium profilinin tutulacağı klasörü üretir.
    /// </summary>
    private static string GetBrowserProfileDirectory()
    {
        var localAppData = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);

        if (string.IsNullOrWhiteSpace(localAppData))
        {
            throw new InvalidOperationException(
                "Windows LocalAppData klasörü bulunamadı.");
        }

        var profileDirectory = Path.Combine(
            localAppData,
            "TTwen",
            "Playwright",
            "Profile");

        Directory.CreateDirectory(profileDirectory);
        return profileDirectory;
    }
}