using Microsoft.UI.Xaml;
using TTwen.App.Services;

namespace TTwen.App;

/// <summary>
/// WinUI uygulamasının yaşam döngüsünü başlatır.
/// </summary>
public partial class App : Microsoft.UI.Xaml.Application
{
    private MainWindow? _window;

    /// <summary>Uygulama servislerinin composition root'udur.</summary>
    public static AppServiceProvider Services { get; } = new();

    /// <summary>App nesnesini oluşturur.</summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>Minimal pencereyi başlatır.</summary>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}
