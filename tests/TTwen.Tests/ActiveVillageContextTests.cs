using TTwen.Application.Services;
using TTwen.Domain.Snapshots;
using Xunit;

namespace TTwen.Tests;

/// <summary>
/// Uygulama genelindeki aktif köy seçim durumunu doğrular.
/// </summary>
public sealed class ActiveVillageContextTests
{
    /// <summary>Seçilen köyün context içinde tutulduğunu doğrular.</summary>
    [Fact]
    public void SetCurrentStoresVillage()
    {
        var context = new ActiveVillageContext();
        var village = CreateVillage("7", "Başkent");

        context.SetCurrent(village);

        Assert.Same(village, context.CurrentVillage);
    }

    /// <summary>Seçim değiştiğinde Changed olayının yayınlandığını doğrular.</summary>
    [Fact]
    public void SetCurrentRaisesChanged()
    {
        var context = new ActiveVillageContext();
        var changed = 0;

        context.Changed += (_, _) => changed++;

        context.SetCurrent(CreateVillage("7", "Başkent"));
        context.SetCurrent(CreateVillage("8", "Yeni Köy"));

        Assert.Equal(2, changed);
    }

    /// <summary>Clear aktif köyü kaldırdığını doğrular.</summary>
    [Fact]
    public void ClearRemovesCurrentVillage()
    {
        var context = new ActiveVillageContext();
        context.SetCurrent(CreateVillage("7", "Başkent"));

        context.Clear();

        Assert.Null(context.CurrentVillage);
    }

    private static VillageSnapshot CreateVillage(string id, string name)
    {
        return new VillageSnapshot(
            id,
            name,
            null,
            100,
            false,
            default,
            default,
            false,
            false,
            DateTimeOffset.UtcNow,
            "https://example.invalid/dorf1.php");
    }
}