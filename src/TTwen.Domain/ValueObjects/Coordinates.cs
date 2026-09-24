namespace TTwen.Domain.ValueObjects;

/// <summary>
/// Haritadaki x/y koordinat çiftini temsil eder.
/// </summary>
/// <remarks>
/// Koordinatın value object olması, mesafe hesabı ve metin gösterimini tek yerde
/// toplar; x ve y'nin ayrı ayrı yanlış aktarılma riskini azaltır.
/// </remarks>
public readonly record struct Coordinates(int X, int Y)
{
    public double DistanceTo(Coordinates other)
    {
        var dx = X - other.X;
        var dy = Y - other.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public override string ToString() => $"({X}|{Y})";
}
