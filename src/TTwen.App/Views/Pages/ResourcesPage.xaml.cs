using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using TTwen.App.Services;
using TTwen.Application.Services;
using TTwen.Domain.Enums;
using TTwen.Domain.Snapshots;

namespace TTwen.App.Views.Pages;

/// <summary>
/// Seçili TTWars köyünün canlı kaynak durumunu gösteren kontrol merkezi sayfasıdır.
/// </summary>
public sealed partial class ResourcesPage : Page
{
    private AppServiceProvider? _services;

    /// <summary>UI'da gösterilen dört kaynak satırının koleksiyonudur.</summary>
    public ObservableCollection<ResourceRow> Resources { get; } = new();

    /// <summary>Kaynak sayfasını oluşturur.</summary>
    public ResourcesPage()
    {
        InitializeComponent();
        ResourceList.ItemsSource = Resources;
    }

    /// <inheritdoc />
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        _services = e.Parameter as AppServiceProvider;

        if (_services is null)
        {
            SetStatus("Uygulama servisleri bulunamadı.", false);
            return;
        }

        UpdateSelectedVillageText();

        if (_services.TTWarsConnection.IsConnected
            && _services.ActiveVillage.CurrentVillage is not null)
        {
            _ = RefreshResourcesAsync();
        }
        else
        {
            SetStatus(
                _services.TTWarsConnection.IsConnected
                    ? "Önce Köyler ekranından bir köy seçin."
                    : "TTWars bağlantısı açık değil.",
                false);
        }
    }

    /// <inheritdoc />
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        _services = null;
    }

    /// <summary>Kullanıcının manuel kaynak okuma komutunu işler.</summary>
    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await RefreshResourcesAsync();
    }

    /// <summary>Seçili köyün canlı kaynak snapshot'ını yeniler.</summary>
    private async Task RefreshResourcesAsync()
    {
        if (_services is null)
            return;

        var selectedVillage = _services.ActiveVillage.CurrentVillage;

        if (selectedVillage is null)
        {
            SetStatus("Önce Köyler ekranından bir köy seçin.", false);
            return;
        }

        if (!_services.TTWarsConnection.IsConnected)
        {
            SetStatus("TTWars bağlantısı açık değil.", false);
            return;
        }

        SetBusyState(true);
        SetStatus("Kaynaklar okunuyor...", true);

        try
        {
            var result = await _services.TTWarsConnection.ReadVillageAsync(
                selectedVillage.Id,
                CancellationToken.None);

            if (result.Villages.Count == 0)
            {
                SetStatus(result.Message, false);
                DetailsText.Text = result.Details ?? string.Empty;
                return;
            }

            var snapshot = result.Villages[0];
            _services.ActiveVillage.SetCurrent(snapshot);

            SelectedVillageText.Text = FormatSelectedVillage(snapshot);
            WarehouseText.Text = FormatCapacity(snapshot.Resources.WarehouseCapacity);
            GranaryText.Text = FormatCapacity(snapshot.Resources.GranaryCapacity);
            LastReadText.Text = snapshot.CapturedAtUtc
                .ToLocalTime()
                .ToString(
                    "dd.MM.yyyy HH:mm:ss",
                    CultureInfo.GetCultureInfo("tr-TR"));

            Resources.Clear();

            foreach (var resource in VillageResourceStatusCalculator.Analyze(snapshot))
                Resources.Add(new ResourceRow(resource));

            SetStatus(result.Message, result.Success);
            DetailsText.Text = result.Details ?? "Tanılayıcı veri yok.";
        }
        catch (OperationCanceledException)
        {
            SetStatus("Kaynak okuma işlemi iptal edildi.", false);
        }
        catch (Exception exception)
        {
            SetStatus(
                "Kaynaklar okunurken beklenmeyen bir hata oluştu: " + exception.Message,
                false);
        }
        finally
        {
            SetBusyState(false);
        }
    }

    /// <summary>Seçili köy bilgisini başlık alanında gösterir.</summary>
    private void UpdateSelectedVillageText()
    {
        var village = _services?.ActiveVillage.CurrentVillage;

        SelectedVillageText.Text = village is null
            ? "Köy seçilmedi"
            : FormatSelectedVillage(village);
    }

    /// <summary>Seçili köy adını ve koordinatlarını biçimler.</summary>
    private static string FormatSelectedVillage(VillageSnapshot village)
    {
        return village.Coordinates is { } coordinates
            ? $"Seçili köy: {village.Name}  {coordinates}"
            : $"Seçili köy: {village.Name}";
    }

    /// <summary>Kapasiteyi kullanıcı dostu biçimde gösterir.</summary>
    private static string FormatCapacity(int? capacity)
    {
        return capacity.HasValue
            ? capacity.Value.ToString("N0", CultureInfo.GetCultureInfo("tr-TR"))
            : "—";
    }

    /// <summary>Sayfanın durum metnini günceller.</summary>
    private void SetStatus(string message, bool success)
    {
        StatusText.Text = message;
        StatusText.Foreground = success
            ? (Brush)Microsoft.UI.Xaml.Application.Current.Resources["TtwenSuccessBrush"]
            : (Brush)Microsoft.UI.Xaml.Application.Current.Resources["TtwenWarningBrush"];
    }

    /// <summary>Okuma sırasında butonu kilitler veya tekrar açar.</summary>
    private void SetBusyState(bool busy)
    {
        RefreshButton.IsEnabled = !busy;
    }

    /// <summary>Kaynak durumu için UI satır modelidir.</summary>
    public sealed class ResourceRow
    {
        /// <summary>Kaynağın kullanıcıya gösterilen adıdır.</summary>
        public string Name { get; }

        /// <summary>Mevcut stok ile kapasiteyi birlikte gösterir.</summary>
        public string StockText { get; }

        /// <summary>Doluluk yüzdesidir; bilinmiyorsa sıfır gösterilir.</summary>
        public double FillPercent { get; }

        /// <summary>Biçimlendirilmiş doluluk oranıdır.</summary>
        public string FillText { get; }

        /// <summary>Saatlik üretim metnidir.</summary>
        public string ProductionText { get; }

        /// <summary>Kapasite metnidir.</summary>
        public string CapacityText { get; }

        /// <summary>Kaynağın durum açıklamasıdır.</summary>
        public string StatusText { get; }

        /// <summary>Doluluk metninin UI fırçasıdır.</summary>
        public Brush FillBrush { get; }

        /// <summary>Durum metninin UI fırçasıdır.</summary>
        public Brush StatusBrush { get; }

        /// <summary>Domain kaynak durumunu UI satırına dönüştürür.</summary>
        public ResourceRow(VillageResourceStatus status)
        {
            Name = GetName(status.ResourceType);
            StockText = FormatStock(status.Current, status.Capacity);
            FillPercent = status.FillPercentage.HasValue
                ? (double)status.FillPercentage.Value
                : 0;
            FillText = status.FillPercentage.HasValue
                ? $"{status.FillPercentage.Value:0.0}%"
                : "—";
            ProductionText = status.PerHour.HasValue
                ? $"+{status.PerHour.Value.ToString("N0", CultureInfo.GetCultureInfo("tr-TR"))} / saat"
                : "Üretim: —";
            CapacityText = status.Capacity.HasValue
                ? $"Kapasite: {status.Capacity.Value.ToString("N0", CultureInfo.GetCultureInfo("tr-TR"))}"
                : "Kapasite: bilinmiyor";

            StatusText = GetStatusText(status);
            FillBrush = GetBrush(status);
            StatusBrush = GetStatusBrush(status);
        }

        /// <summary>Kaynağı Türkçe kullanıcı metnine dönüştürür.</summary>
        private static string GetName(ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Wood => "ODUN",
                ResourceType.Clay => "TUĞLA",
                ResourceType.Iron => "DEMİR",
                ResourceType.Crop => "TAHIL",
                _ => resourceType.ToString()
            };
        }

        /// <summary>Stok ve kapasite bilgisini biçimler.</summary>
        private static string FormatStock(int? current, int? capacity)
        {
            if (current.HasValue && capacity.HasValue)
            {
                return $"{current.Value.ToString("N0", CultureInfo.GetCultureInfo("tr-TR"))}"
                     + " / "
                     + $"{capacity.Value.ToString("N0", CultureInfo.GetCultureInfo("tr-TR"))}";
            }

            return current.HasValue
                ? current.Value.ToString("N0", CultureInfo.GetCultureInfo("tr-TR"))
                : "Veri yok";
        }

        /// <summary>Durum rozetinin metnini belirler.</summary>
        private static string GetStatusText(VillageResourceStatus status)
        {
            if (!status.Current.HasValue)
                return "VERİ YOK";

            if (!status.Capacity.HasValue)
                return "KAPASİTE BİLİNMİYOR";

            if (status.IsOverflowRisk)
                return "TAŞMA RİSKİ";

            if (status.IsLowStock)
                return "DÜŞÜK STOK";

            return "NORMAL";
        }

        /// <summary>Doluluk göstergesinin fırçasını belirler.</summary>
        private static Brush GetBrush(VillageResourceStatus status)
        {
            var key = status.IsOverflowRisk || status.IsLowStock
                ? "TtwenWarningBrush"
                : "TtwenAccentBrush";

            return (Brush)Microsoft.UI.Xaml.Application.Current.Resources[key];
        }

        /// <summary>Durum metninin fırçasını belirler.</summary>
        private static Brush GetStatusBrush(VillageResourceStatus status)
        {
            var key = status.IsOverflowRisk || status.IsLowStock
                ? "TtwenWarningBrush"
                : "TtwenSuccessBrush";

            return (Brush)Microsoft.UI.Xaml.Application.Current.Resources[key];
        }
    }
}