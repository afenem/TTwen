using Xunit;
using TTwen.Domain.ValueObjects;

namespace TTwen.Tests;

/// <summary>
/// Coordinates value object'inin mesafe hesabını doğrular.
/// </summary>
public sealed class CoordinatesTests
{
    [Fact]
    public void DistanceToReturnsExpectedEuclideanDistance()
    {
        var first = new Coordinates(0, 0);
        var second = new Coordinates(3, 4);

        Assert.Equal(5, first.DistanceTo(second), 5);
    }
}