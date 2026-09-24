using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TTwen.App.Services;
using TTwen.App.Views.Pages;

namespace TTwen.App;

/// <summary>
/// Ana pencerenin uygulama kabuğudur.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly AppServiceProvider _services;

    /// <summary>Uygulama shell'ini oluşturur.</summary>
    public MainWindow()
    {
        _services = App.Services;

        InitializeComponent();
        Closed += (_, _) => _ = _services.DisposeAsync().AsTask();

        ContentFrame.Navigate(typeof(DashboardPage));
    }

    /// <summary>Sol navigasyon seçimlerini ilgili modül sayfasına yönlendirir.</summary>
    private void MainNavigation_SelectionChanged(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is not NavigationViewItem item)
            return;

        switch (item.Tag?.ToString())
        {
            case "Dashboard":
                ContentFrame.Navigate(typeof(DashboardPage));
                break;

            case "Server":
                ContentFrame.Navigate(
                    typeof(TTWarsServerPage),
                    _services);
                break;

            case "Villages":
                ContentFrame.Navigate(
                    typeof(VillagesPage),
                    _services);
                break;

            case "Buildings":
                ContentFrame.Navigate(
                    typeof(BuildingsPage),
                    _services);
                break;

            default:
                ContentFrame.Navigate(
                    typeof(ModulePlaceholderPage),
                    item.Content?.ToString());
                break;
        }
    }
}