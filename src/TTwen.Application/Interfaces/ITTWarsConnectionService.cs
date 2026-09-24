using TTwen.Application.Models;

namespace TTwen.Application.Interfaces;

/// <summary>
/// TTWars tarayıcı bağlantısının application katmanına sunduğu sözleşmedir.
/// </summary>
/// <remarks>
/// UI bu sözleşmeyi kullanır; Playwright veya TTWars HTML ayrıntılarını bilmez.
/// </remarks>
public interface ITTWarsConnectionService : IAsyncDisposable
{
    /// <summary>
    /// Geçerli bir tarayıcı bağlantısı olup olmadığını bildirir.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Bağlı olunan TTWars adresini döndürür.
    /// </summary>
    string? CurrentServerUrl { get; }

    /// <summary>
    /// Son açılan sayfanın başlığını döndürür.
    /// </summary>
    string? CurrentPageTitle { get; }

    /// <summary>
    /// Verilen TTWars adresine kalıcı tarayıcı oturumu ile bağlanır.
    /// </summary>
    Task<TTWarsConnectionResult> ConnectAsync(
        string serverUrl,
        CancellationToken cancellationToken);

    /// <summary>
    /// Tarayıcı oturumunu kapatır ve bağlantı durumunu sıfırlar.
    /// </summary>
    Task DisconnectAsync();
}
