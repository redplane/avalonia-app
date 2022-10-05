using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EagleEye.Apps.Mobile.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private WebViewSource _webViewWebPageSource;

    public WebViewSource WebPageSource
    {
        get => _webViewWebPageSource;
        set
        {
            if (!Equals(_webViewWebPageSource, value))
            {
                _webViewWebPageSource = value;
                OnPropertyChanged();
            }
        }
    }

    public MainPageViewModel()
    {
        _webViewWebPageSource = "https://www.google.com";
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}