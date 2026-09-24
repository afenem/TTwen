namespace TTwen.Application.Models;

/// <summary>
/// TTWars bağlantı denemesinin kullanıcı arayüzüne aktarılacak sonucudur.
/// </summary>
public sealed record TTWarsConnectionResult(
    bool Success,
    string Message,
    string? ServerUrl,
    string? PageTitle,
    string? Details);
