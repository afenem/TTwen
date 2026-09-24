namespace TTwen.Core.Interfaces;

/// <summary>
/// Zaman bilgisini uygulama kodundan soyutlar.
/// </summary>
/// <remarks>
/// Zamanı doğrudan DateTime.Now ile kullanmak yerine abstraction kullanılır;
/// bunun nedeni zaman bağımlı davranışları testlerde sabitleyebilmektir.
/// </remarks>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
