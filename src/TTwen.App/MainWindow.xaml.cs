using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TTwen.App.Views.Pages;

namespace TTwen.App;

/// <summary>
/// Ana pencerenin uygulama kabuğudur.
/// </summary>
/// <remarks>
/// Navigation ve ortak görünüm burada tutulur. Modüllerin iş mantığı bu sınıfa
/// konmaz; her modül kendi Page/ViewModel katmanında geliştirilir.
/// </remarks>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ContentFrame.Navigate(typeof(DashboardPage));
    }

    private void MainNavigation_SelectionChanged(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is not NavigationViewItem item)
            return;

        if (item.Tag?.ToString() == "Dashboard")
        {
            ContentFrame.Navigate(typeof(DashboardPage));
            return;
        }

        ContentFrame.Navigate(
            typeof(ModulePlaceholderPage),
            item.Content?.ToString());
    }
}