using Microsoft.UI.Xaml;
using TTwen.App.Services;

namespace TTwen.App;

/// <summary>
/// WinUI uygulamasının yaşam döngüsünü başlatır.
/// </summary>
/// <remarks>
/// İş mantığı burada bulunmaz. Önce pencereyi aktive eder, ardından UI dispatcher
/// üzerinden varsayılan sayfayı yükler. Böylece pencere oluşturma ile sayfa
/// XAML'inin ilk yüklenmesi birbirinden ayrılır.
/// </remarks>
public partial class App : Microsoft.UI.Xaml.Application
{
    private MainWindow? _window;

    /// <summary>Uygulama genelindeki servislerin composition root'udur.</summary>
    public static AppServiceProvider Services { get; } = new();

    /// <summary>App nesnesini oluşturur.</summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>Uygulama ilk kez çalıştırıldığında ana pencereyi başlatır.</summary>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();

        _window.DispatcherQueue.TryEnqueue(
            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
            () => _window.InitializeDefaultPage());
    }
}
