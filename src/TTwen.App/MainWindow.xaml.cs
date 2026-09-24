using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TTwen.App.Services;
using TTwen.App.Views.Pages;

namespace TTwen.App;

/// <summary>
/// TTwen kontrol merkezinin ana pencere kabuğudur.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly AppServiceProvider _services;

    /// <summary>Kontrol merkezi ana penceresini oluşturur.</summary>
    public MainWindow()
    {
        _services = App.Services;

        InitializeComponent();
        Closed += (_, _) => _ = _services.DisposeAsync().AsTask();

        ShowStartScreen();
    }

    /// <summary>
    /// Uygulama açılışında hafif bir başlangıç ekranı gösterir.
    /// </summary>
    private void ShowStartScreen()
    {
        ContentFrame.Content = new Grid
        {
            Padding = new Thickness(28),
            Children =
            {
                new StackPanel
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "TTWEN",
                            FontSize = 36,
                            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                            Foreground = (Microsoft.UI.Xaml.Media.Brush)Microsoft.UI.Xaml.Application.Current.Resources["TtwenAccentBrush"],
                            HorizontalAlignment = HorizontalAlignment.Center
                        },
                        new TextBlock
                        {
                            Text = "TTWARS CONTROL CENTER",
                            Margin = new Thickness(0, 8, 0, 0),
                            FontSize = 14,
                            Foreground = (Microsoft.UI.Xaml.Media.Brush)Microsoft.UI.Xaml.Application.Current.Resources["TtwenSuccessBrush"],
                            HorizontalAlignment = HorizontalAlignment.Center
                        },
                        new TextBlock
                        {
                            Text = "Sol menüden bir modül seçin.",
                            Margin = new Thickness(0, 14, 0, 0),
                            FontSize = 12,
                            Foreground = (Microsoft.UI.Xaml.Media.Brush)Microsoft.UI.Xaml.Application.Current.Resources["TtwenWarningBrush"],
                            HorizontalAlignment = HorizontalAlignment.Center
                        }
                    }
                }
            }
        };
    }

    /// <summary>Sol menüdeki modül butonlarını ilgili sayfaya yönlendirir.</summary>
    private void NavigationButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        switch (button.Tag?.ToString())
        {
            case "Dashboard":
                ContentFrame.Navigate(typeof(DashboardPage));
                break;
            case "Server":
                ContentFrame.Navigate(typeof(TTWarsServerPage), _services);
                break;
            case "Villages":
                ContentFrame.Navigate(typeof(VillagesPage), _services);
                break;
            case "Buildings":
                ContentFrame.Navigate(typeof(BuildingsPage), _services);
                break;
            case "Resources":
                ContentFrame.Navigate(typeof(ResourcesPage), _services);
                break;
            default:
                ContentFrame.Navigate(typeof(ModulePlaceholderPage), button.Content?.ToString());
                break;
        }
    }
}
