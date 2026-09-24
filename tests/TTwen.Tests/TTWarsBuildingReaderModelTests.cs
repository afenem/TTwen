using TTwen.Domain.Snapshots;
using Xunit;

namespace TTwen.Tests;

/// <summary>
/// Bina snapshot modelinin temel durumlarını doğrular.
/// </summary>
public sealed class TTWarsBuildingReaderModelTests
{
    /// <summary>Doluluk ve inşaat durumunun korunabildiğini doğrular.</summary>
    [Fact]
    public void BuildingSnapshotPreservesState()
    {
        var snapshot = new BuildingSnapshot(
            SlotId: 26,
            Name: "Kışla",
            Level: 5,
            Gid: 19,
            IsOccupied: true,
            IsUnderConstruction: true,
            SourceUrl: "https://example.invalid/dorf2.php");

        Assert.Equal(26, snapshot.SlotId);
        Assert.Equal("Kışla", snapshot.Name);
        Assert.Equal(5, snapshot.Level);
        Assert.Equal(19, snapshot.Gid);
        Assert.True(snapshot.IsOccupied);
        Assert.True(snapshot.IsUnderConstruction);
    }

    /// <summary>Boş bina arsasının bilinmeyen seviyeden ayrıldığını doğrular.</summary>
    [Fact]
    public void EmptySlotCanHaveUnknownLevelAndGid()
    {
        var snapshot = new BuildingSnapshot(
            SlotId: 31,
            Name: "Bina arsası",
            Level: null,
            Gid: null,
            IsOccupied: false,
            IsUnderConstruction: false,
            SourceUrl: "https://example.invalid/dorf2.php");

        Assert.False(snapshot.IsOccupied);
        Assert.Null(snapshot.Level);
        Assert.Null(snapshot.Gid);
    }
}