using TTwen.Domain.Enums;
using TTwen.Domain.ValueObjects;

namespace TTwen.Domain.Entities;

/// <summary>
/// Tek bir vahanın domain modelidir.
/// </summary>
/// <remarks>
/// Bu nesne yalnızca oyun bilgisini temsil eder. HTML, Playwright veya UI
/// ayrıntısı taşımaz; bu sayede simülatör web sitesinden bağımsızdır.
/// </remarks>
public sealed class Oasis
{
    private readonly Dictionary<AnimalType, int> _animals;

    public Coordinates Coordinates { get; }
    public IReadOnlyDictionary<AnimalType, int> Animals => _animals;
    public string BonusDescription { get; }
    public double? DistanceFromVillage { get; set; }

    public Oasis(
        Coordinates coordinates,
        IDictionary<AnimalType, int> animals,
        string bonusDescription = "")
    {
        Coordinates = coordinates;
        _animals = new Dictionary<AnimalType, int>(animals);
        BonusDescription = bonusDescription;
    }

    /// <summary>
    /// Vahadaki toplam doğa birimi sayısını verir.
    /// </summary>
    public int TotalAnimals => _animals.Values.Sum();
}
