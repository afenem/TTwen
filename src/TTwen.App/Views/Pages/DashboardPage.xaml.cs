using Microsoft.UI.Xaml.Controls;

namespace TTwen.App.Views.Pages;

/// <summary>
/// Dashboard sayfasının yaşam döngüsü sınıfıdır.
/// </summary>
/// <remarks>
/// Hesaplama burada yapılmaz. Gerçek veriler ViewModel ve Application servislerinden
/// gelir; bu dosya yalnızca XAML yaşam döngüsünü taşır.
/// </remarks>
public sealed partial class DashboardPage : Page
{
    public DashboardPage()
    {
        InitializeComponent();
    }
}