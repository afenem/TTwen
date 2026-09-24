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
public sealed partial class VillagesPage : Page
{
    private AppServiceProvider? _services;

    /// <summary>UI'da gösterilen köy satırlarının koleksiyonudur.</summary>
    public ObservableCollection<VillageRow> Villages { get; } = new();

    /// <summary>Köy sayfasını oluşturur.</summary>
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
            _ = RefreshVillagesAsync();
    }

    /// <summary>Kullanıcının manuel yenileme komutunu işler.</summary>
    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await RefreshVillagesAsync();
    }

    /// <summary>Listeden seçilen köyü uygulamanın ortak aktif köyü yapar.</summary>
    private void VillageList_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (_services is null || VillageList.SelectedItem is not VillageRow row)
            return;

        _services.ActiveVillage.SetCurrent(row.Snapshot);
        SelectedVillageText.Text = FormatSelectedVillage(row.Snapshot);
    }

    /// <summary>Canlı TTWars köy snapshot'larını okuyup ekranda yeniler.</summary>
    private async Task RefreshVillagesAsync()
    {
        if (_services is null)
            return;

        if (!_services.TTWarsConnection.IsConnected)
        {
            SetStatus(
                "TTWars bağlantısı açık değil.",
                "Önce TTWars Sunucu ekranından bağlantı kurun.",
                false);
            return;
        }

        SetBusyState(true);
        SetStatus("Köyler okunuyor...", null, true);

        try
        {
            var result = await _services.TTWarsConnection.ReadVillagesAsync(
                CancellationToken.None);

            Villages.Clear();

            foreach (var village in result.Villages)
                Villages.Add(new VillageRow(village));

            VillageCountText.Text = Villages.Count.ToString(
                CultureInfo.InvariantCulture);

            LastReadText.Text = result.CapturedAtUtc
                .ToLocalTime()
                .ToString(
                    "dd.MM.yyyy HH:mm:ss",
                    CultureInfo.GetCultureInfo("tr-TR"));

            var currentId = _services.ActiveVillage.CurrentVillage?.Id;
            var selected = Villages.FirstOrDefault(row => row.Id == currentId)
                ?? Villages.FirstOrDefault();

            VillageList.SelectedItem = selected;

            if (selected is not null)
            {
                _services.ActiveVillage.SetCurrent(selected.Snapshot);
                SelectedVillageText.Text = FormatSelectedVillage(selected.Snapshot);
            }
            else
            {
                _services.ActiveVillage.Clear();
                SelectedVillageText.Text = "—";
            }

            SetStatus(result.Message, result.Details, result.Success);
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

    /// <summary>Seçili köyün kısa gösterimini üretir.</summary>
    private static string FormatSelectedVillage(VillageSnapshot snapshot)
    {
        return snapshot.Coordinates is { } coordinates
            ? $"{snapshot.Name}  {coordinates}"
            : snapshot.Name;
    }

    /// <summary>Sayfanın durum alanlarını günceller.</summary>
    private void SetStatus(string message, string? details, bool success)
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

    /// <summary>Okuma sırasında kullanıcı kontrollerini kilitler veya tekrar açar.</summary>
    private void SetBusyState(bool busy)
    {
        RefreshButton.IsEnabled = !busy;
    }
}