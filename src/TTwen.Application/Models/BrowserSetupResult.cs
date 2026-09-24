namespace TTwen.Application.Models;

/// <summary>
/// Chromium hazırlama işleminin sonucudur.
/// </summary>
public sealed record BrowserSetupResult(
    bool Success,
    string Message,
    string? Details);
