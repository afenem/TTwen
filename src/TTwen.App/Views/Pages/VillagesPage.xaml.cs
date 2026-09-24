using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using TTwen.App.Services;
using TTwen.Domain.Snapshots;

namespace TTwen.App.Views.Pages;

/// <summary>
/// TTWars hesabındaki köylerin canlı durumlarını gösteren kontrol merkezi sayfasıdır.
/// </summary>
/// <remarks>
/// Sayfa yalnızca Application servisi üzerinden veri ister. Playwright ve HTML selector
/// ayrıntıları UI katmanına taşınmaz.
/// </remarks>
public sealed partial class VillagesPage : Page
{
    private AppServiceProvider? _services;

    /// <summary>
    /// UI'da gösterilen köy satırlarının koleksiyonudur.
    /// </summary>
    public ObservableCollection<VillageRow> Villages { get; } = new();

    /// <summary>
    /// Köy sayfasını oluşturur.
    /// </summary>
    public VillagesPage()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        _services = e.Parameter as AppServiceProvider;

        if (_services is not null)
        {
            _ = RefreshVillagesAsync();
        }
    }

    /// <summary>
    /// Kullanıcının manuel yenileme komutunu işler.
    /// </summary>
    private async void RefreshButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await RefreshVillagesAsync();
    }

    /// <summary>
    /// Canlı TTWars köy snapshot'larını okuyup ekranda yeniler.
    /// </summary>
    private async Task RefreshVillagesAsync()
    {
        if (_services is null)
            return;

        if (!_services.TTWarsConnection.IsConnected)
        {
            SetStatus(
                "TTWars bağlantısı açık değil.",
                "Önce TTWars Sunucu ekranından bağlantı kurun.",
                success: false);

            return;
        }

        SetBusyState(true);
        SetStatus("Köyler okunuyor...", null, success: true);

        try
        {
            var result = await _services.TTWarsConnection.ReadVillagesAsync(
                CancellationToken.None);

            Villages.Clear();

            foreach (var village in result.Villages)
            {
                Villages.Add(new VillageRow(village));
            }

            VillageCountText.Text = Villages.Count.ToString(
                CultureInfo.InvariantCulture);

            LastReadText.Text = result.CapturedAtUtc
                .ToLocalTime()
                .ToString(
                    "dd.MM.yyyy HH:mm:ss",
                    CultureInfo.GetCultureInfo("tr-TR"));

            SetStatus(
                result.Message,
                result.Details,
                result.Success);

            VillageList.UpdateLayout();
        }
        catch (OperationCanceledException)
        {
            SetStatus("Köy okuma işlemi iptal edildi.", null, false);
        }
        catch (Exception exception)
        {
            SetStatus(
                "Köyler okunurken beklenmeyen bir hata oluştu.",
                exception.Message,
                false);
        }
        finally
        {
            SetBusyState(false);
        }
    }

    /// <summary>
    /// Sayfanın durum alanlarını günceller.
    /// </summary>
    private void SetStatus(
        string message,
        string? details,
        bool success)
    {
        StatusText.Text = message;
        StatusText.Foreground = success
            ? (Brush)Microsoft.UI.Xaml.Application.Current.Resources["TtwenSuccessBrush"]
            : (Brush)Microsoft.UI.Xaml.Application.Current.Resources["TtwenWarningBrush"];

        DetailsText.Text = details ?? string.Empty;
        DetailsPanel.Visibility = string.IsNullOrWhiteSpace(details)
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    /// <summary>
    /// Okuma sırasında kullanıcı kontrollerini kilitler veya tekrar açar.
    /// </summary>
    private void SetBusyState(bool busy)
    {
        RefreshButton.IsEnabled = !busy;
    }

    /// <summary>
    /// Bir domain köy snapshot'ını tablo satırında gösterilecek metinlere dönüştürür.
    /// </summary>
    public sealed class VillageRow
    {
        /// <summary>Satırda gösterilen köy adıdır.</summary>
        public string Name { get; }

        /// <summary>Satırda gösterilen köy kimliğidir.</summary>
        public string Id { get; }

        /// <summary>Koordinat gösterimidir.</summary>
        public string CoordinatesText { get; }

        /// <summary>Nüfus gösterimidir.</summary>
        public string PopulationText { get; }

        /// <summary>Dört kaynağın anlık stok gösterimidir.</summary>
        public string ResourcesText { get; }

        /// <summary>Depo ve ambar kapasitesinin gösterimidir.</summary>
        public string StorageText { get; }

        /// <summary>Saatlik üretim değerlerinin gösterimidir.</summary>
        public string ProductionText { get; }

        /// <summary>Snapshot'ın veri kalitesini özetleyen kısa durumdur.</summary>
        public string StatusText { get; }

        /// <summary>Domain snapshot'ından UI satırı oluşturur.</summary>
        public VillageRow(VillageSnapshot snapshot)
        {
            Name = snapshot.IsCapital == true
                ? $"{snapshot.Name} • BAŞKENT"
                : snapshot.Name;

            Id = snapshot.Id;

            CoordinatesText = snapshot.Coordinates is { } coordinates
                ? coordinates.ToString()
                : "—";

            PopulationText = Format(snapshot.Population);

            ResourcesText =
                $"O {Format(snapshot.Resources.Wood)} · "
                + $"T {Format(snapshot.Resources.Clay)} · "
                + $"D {Format(snapshot.Resources.Iron)} · "
                + $"H {Format(snapshot.Resources.Crop)}";

            StorageText =
                $"Depo {Format(snapshot.Resources.WarehouseCapacity)} · "
                + $"Ambar {Format(snapshot.Resources.GranaryCapacity)}";

            ProductionText =
                $"O {Format(snapshot.Production.WoodPerHour)} · "
                + $"T {Format(snapshot.Production.ClayPerHour)} · "
                + $"D {Format(snapshot.Production.IronPerHour)} · "
                + $"H {Format(snapshot.Production.CropPerHour)}";

            var missing = new List<string>();

            if (!snapshot.Coordinates.HasValue)
                missing.Add("koordinat yok");

            if (!snapshot.HasResourceData)
                missing.Add("kaynak işareti yok");

            if (!snapshot.HasProductionData)
                missing.Add("üretim yok");

            StatusText = missing.Count == 0
                ? "CANLI OKUMA"
                : string.Join(" • ", missing);
        }

        /// <summary>Nullable sayıları Türkçe binlik ayırıcıyla gösterir.</summary>
        private static string Format(int? value)
        {
            return value?.ToString(
                    "N0",
                    CultureInfo.GetCultureInfo("tr-TR"))
                ?? "—";
        }
    }
}