using Microsoft.UI.Xaml.Controls;

using multivpn.ViewModels;

namespace multivpn.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }
}
