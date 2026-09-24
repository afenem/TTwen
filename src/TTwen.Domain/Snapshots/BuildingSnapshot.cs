namespace TTwen.Domain.Snapshots;

/// <summary>
/// TTWars köy merkezinden tek bir bina slotunun canlı durumunu temsil eder.
/// </summary>
/// <remarks>
/// Level veya Gid null ise ilgili HTML alanının güvenilir biçimde okunamadığı anlaşılır.
/// IsOccupied false ise slotun bina alanı olduğu, ancak henüz yapı kurulmadığı kabul edilir.
/// </remarks>
public sealed record BuildingSnapshot(
    int SlotId,
    string Name,
    int? Level,
    int? Gid,
    bool IsOccupied,
    bool IsUnderConstruction,
    string SourceUrl);