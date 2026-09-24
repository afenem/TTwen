using Microsoft.UI.Xaml;
using TTwen.App.Services;

namespace TTwen.App;

/// <summary>
/// Ana pencerenin geçici başlangıç izolasyon kabuğudur.
/// </summary>
/// <remarks>
/// Bu sürüm, yayınlanmış WinUI başlatmasının temel XAML kabuğundan bağımsız
/// çalışabildiğini doğrulamak için Dashboard ve NavigationView yüklemez.
/// </remarks>
public sealed partial class MainWindow : Window
{
    private readonly AppServiceProvider _services;

    /// <summary>Minimal başlangıç penceresini oluşturur.</summary>
    public MainWindow()
    {
        _services = App.Services;
        InitializeComponent();
        Closed += (_, _) => _ = _services.DisposeAsync().AsTask();
    }
}
