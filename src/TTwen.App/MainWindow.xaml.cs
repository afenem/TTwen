using Microsoft.UI.Xaml;
using TTwen.App.Services;

namespace TTwen.App;

/// <summary>
/// WinUI başlangıç izolasyon penceresidir.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly AppServiceProvider _services;

    /// <summary>Temel pencereyi oluşturur.</summary>
    public MainWindow()
    {
        _services = App.Services;
        InitializeComponent();
        Closed += (_, _) => _ = _services.DisposeAsync().AsTask();
    }
}
