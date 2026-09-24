using TTwen.Domain.Snapshots;

namespace TTwen.Application.Models;

/// <summary>
/// Aktif TTWars köyünün bina okuma işleminin sonucunu taşır.
/// </summary>
public sealed record TTWarsBuildingReadResult(
    bool Success,
    IReadOnlyList<BuildingSnapshot> Buildings,
    string Message,
    string? Details,
    DateTimeOffset CapturedAtUtc,
    string SourceUrl);