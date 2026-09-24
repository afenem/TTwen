using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using TTwen.App.Services;

namespace TTwen.App.Views.Pages;

/// <summary>
/// TTWars sunucu bağlantısının kullanıcı arayüzüdür.
/// </summary>
/// <remarks>
/// Sayfa yalnızca kullanıcı komutlarını application servislerine iletir ve
/// sonuçları görselleştirir. Playwright çağrıları burada doğrudan yapılmaz.
/// </remarks>
public sealed partial class TTWarsServerPage : Page
{
    private AppServiceProvider? _services;

    public TTWarsServerPage()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        _services = e.Parameter as AppServiceProvider;

        if (_services is not null)
        {
            RefreshConnectionState();
        }
    }

    private async void ConnectButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_services is null)
            return;

        SetBusyState(true);
        SetConnectionMessage("TTWars sunucusuna bağlanılıyor.", null);

        try
        {
            var result = await _services.TTWarsConnection.ConnectAsync(
                ServerUrlTextBox.Text,
                CancellationToken.None);

            SetConnectionMessage(result.Message, result.Details);
            CurrentServerText.Text = result.ServerUrl ?? "—";
            CurrentTitleText.Text = result.PageTitle ?? "—";
            RefreshConnectionState();
        }
        catch (OperationCanceledException)
        {
            SetConnectionMessage("Bağlantı işlemi iptal edildi.", null);
            RefreshConnectionState();
        }
        catch (Exception exception)
        {
            SetConnectionMessage(
                "Bağlantı işlemi kullanıcı arayüzünde beklenmeyen bir hatayla sonlandı.",
                exception.Message);
            RefreshConnectionState();
        }
        finally
        {
            SetBusyState(false);
        }
    }

    private async void DisconnectButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_services is null)
            return;

        SetBusyState(true);

        try
        {
            await _services.TTWarsConnection.DisconnectAsync();

            SetConnectionMessage("Bağlantı kapatıldı.", null);
            CurrentServerText.Text = "—";
            CurrentTitleText.Text = "—";
            RefreshConnectionState();
        }
        catch (Exception exception)
        {
            SetConnectionMessage(
                "Bağlantı kapatılırken hata oluştu.",
                exception.Message);
        }
        finally
        {
            SetBusyState(false);
        }
    }

    private async void InstallChromiumButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_services is null)
            return;

        SetBusyState(true);
        BrowserSetupStatusText.Text = "Chromium hazırlanıyor...";

        try
        {
            var result = await _services.BrowserSetup.InstallChromiumAsync(
                CancellationToken.None);

            BrowserSetupStatusText.Text = result.Details is null
                ? result.Message
                : $"{result.Message} {result.Details}";
        }
        catch (Exception exception)
        {
            BrowserSetupStatusText.Text =
                $"Kurulum sırasında hata oluştu: {exception.Message}";
        }
        finally
        {
            SetBusyState(false);
        }
    }

    private void RefreshConnectionState()
    {
        if (_services is null)
            return;

        var connected = _services.TTWarsConnection.IsConnected;

        ConnectionStateText.Text = connected
            ? "BAĞLI"
            : "BAĞLANTI BEKLİYOR";

        ConnectionStateText.Foreground = connected
            ? (Brush)Application.Current.Resources["TtwenSuccessBrush"]
            : (Brush)Application.Current.Resources["TtwenWarningBrush"];

        ConnectionDot.Fill = ConnectionStateText.Foreground;
        DisconnectButton.IsEnabled = connected;
        ConnectButton.IsEnabled = !connected;
    }

    private void SetConnectionMessage(
        string message,
        string? details)
    {
        ConnectionMessageText.Text = message;
        ConnectionDetailsText.Text = details ?? string.Empty;
        ConnectionDetailsText.Visibility = string.IsNullOrWhiteSpace(details)
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void SetBusyState(bool busy)
    {
        ServerUrlTextBox.IsEnabled = !busy;
        ConnectButton.IsEnabled = !busy && !(_services?.TTWarsConnection.IsConnected ?? false);
        DisconnectButton.IsEnabled = !busy && (_services?.TTWarsConnection.IsConnected ?? false);
        InstallChromiumButton.IsEnabled = !busy;
    }
}
