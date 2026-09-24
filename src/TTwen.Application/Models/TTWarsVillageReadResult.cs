using TTwen.Domain.Snapshots;

namespace TTwen.Application.Models;

/// <summary>
/// TTWars köy okuma işleminin sonucunu temsil eder.
/// </summary>
public sealed record TTWarsVillageReadResult(
    bool Success,
    IReadOnlyList<VillageSnapshot> Villages,
    int RequestedCount,
    int FailedCount,
    string Message,
    string? Details,
    DateTimeOffset CapturedAtUtc);