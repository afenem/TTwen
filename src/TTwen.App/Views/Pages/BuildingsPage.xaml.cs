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
/// Aktif TTWars köyünün bina durumunu gösteren kontrol merkezi sayfasıdır.
/// </summary>
public sealed partial class BuildingsPage : Page
{
    private AppServiceProvider? _services;

    /// <summary>UI'daki bina satırlarının koleksiyonudur.</summary>
    public ObservableCollection<BuildingRow> Buildings { get; } = new();

    /// <summary>Bina sayfasını oluşturur.</summary>
    public BuildingsPage()
    {
        InitializeComponent();
        BuildingList.ItemsSource = Buildings;
    }

    /// <inheritdoc />
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        _services = e.Parameter as AppServiceProvider;

        if (_services is not null && _services.TTWarsConnection.IsConnected)
            _ = RefreshBuildingsAsync();
        else
            SetStatus("TTWars bağlantısı açık değil.", success: false);
    }

    /// <summary>Kullanıcının manuel bina okuma komutunu işler.</summary>
    private async void RefreshButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await RefreshBuildingsAsync();
    }

    /// <summary>Aktif köyün canlı bina snapshot'larını alır ve tabloyu yeniler.</summary>
    private async Task RefreshBuildingsAsync()
    {
        if (_services is null)
            return;

        if (!_services.TTWarsConnection.IsConnected)
        {
            SetStatus("TTWars bağlantısı açık değil.", success: false);
            return;
        }

        RefreshButton.IsEnabled = false;
        SetStatus("Binalar okunuyor...", success: true);

        try
        {
            var result = await _services.TTWarsConnection.ReadBuildingsAsync(
                CancellationToken.None);

            Buildings.Clear();

            foreach (var building in result.Buildings)
                Buildings.Add(new BuildingRow(building));

            SlotCountText.Text = Buildings.Count.ToString(
                CultureInfo.InvariantCulture);

            OccupiedCountText.Text = Buildings.Count(
                building => building.IsOccupied).ToString(
                    CultureInfo.InvariantCulture);

            var success = result.Success || result.Buildings.Count > 0;

            SetStatus(result.Message, success);

            if (!string.IsNullOrWhiteSpace(result.Details))
                SetStatus(
                    result.Message + " • " + result.Details,
                    success: false);
        }
        catch (OperationCanceledException)
        {
            SetStatus("Bina okuma işlemi iptal edildi.", false);
        }
        catch (Exception exception)
        {
            SetStatus(
                "Binalar okunurken hata oluştu: " + exception.Message,
                false);
        }
        finally
        {
            RefreshButton.IsEnabled = true;
        }
    }

    /// <summary>UI durum metnini günceller.</summary>
    private void SetStatus(string message, bool success)
    {
        StatusText.Text = message;
        StatusText.Foreground = success
            ? (Brush)Microsoft.UI.Xaml.Application.Current.Resources["TtwenSuccessBrush"]
            : (Brush)Microsoft.UI.Xaml.Application.Current.Resources["TtwenWarningBrush"];
    }

    /// <summary>Domain bina snapshot'ını tablo satırına dönüştürür.</summary>
    public sealed class BuildingRow
    {
        /// <summary>Slot kimliğidir.</summary>
        public string SlotId { get; }

        /// <summary>Bina adıdır.</summary>
        public string Name { get; }

        /// <summary>Bina seviyesidir.</summary>
        public string Level { get; }

        /// <summary>Travian bina grup kimliğidir.</summary>
        public string Gid { get; }

        /// <summary>Canlı bina slotunun dolu olup olmadığını belirtir.</summary>
        public bool IsOccupied { get; }

        /// <summary>Canlı veri durumudur.</summary>
        public string Status { get; }

        /// <summary>Snapshot'ı kullanıcıya gösterilecek satıra dönüştürür.</summary>
        public BuildingRow(BuildingSnapshot snapshot)
        {
            IsOccupied = snapshot.IsOccupied;
            SlotId = snapshot.SlotId.ToString(CultureInfo.InvariantCulture);
            Name = snapshot.IsOccupied ? snapshot.Name : "Bina arsası";
            Level = snapshot.Level?.ToString(CultureInfo.InvariantCulture) ?? "—";
            Gid = snapshot.Gid?.ToString(CultureInfo.InvariantCulture) ?? "—";
            Status = snapshot.IsUnderConstruction
                ? "İNŞA / YÜKSELTME"
                : snapshot.IsOccupied
                    ? "HAZIR"
                    : "BOŞ";
        }
    }
}