using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace TTwen.App.Views.Pages;

/// <summary>
/// Henüz kendi görünümü oluşturulmamış modüllerin geçici görünümüdür.
/// </summary>
/// <remarks>
/// İlk sürümde bütün ana navigasyonu kurmamızı sağlar. Her modül tamamlandıkça
/// bu sayfa gerçek Page + ViewModel ile değiştirilir.
/// </remarks>
public sealed partial class ModulePlaceholderPage : Page
{
    public ModulePlaceholderPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ModuleTitle.Text = e.Parameter?.ToString() ?? "Modül";
    }
}