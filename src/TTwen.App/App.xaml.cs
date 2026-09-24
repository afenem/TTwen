using Microsoft.UI.Xaml;

namespace TTwen.App;

/// <summary>
/// WinUI uygulamasının yaşam döngüsünü başlatır.
/// </summary>
/// <remarks>
/// İş mantığı burada bulunmaz. Sadece ana pencereyi oluşturur; böylece
/// UI yaşam döngüsü business logic'ten ayrılır.
/// </remarks>
public partial class App : Microsoft.UI.Xaml.Application
{
    private MainWindow? _window;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}