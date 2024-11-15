using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using multivpn.Models;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace multivpn.Views;



public sealed partial class AddVPNPage : Page
{
    private StorageFile configFile;

    public AddVPNPage()
    {
        this.InitializeComponent();
    }

    private async void OnSelectFileClick(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".conf");
        picker.FileTypeFilter.Add(".ovpn");
        picker.SuggestedStartLocation = PickerLocationId.Desktop;

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        configFile = await picker.PickSingleFileAsync();
        if (configFile != null)
        {
            (sender as Button).Content = configFile.Name;
        }
    }

    private async void OnSubmitVpn(object sender, RoutedEventArgs e)
    {
        var name = VpnNameTextBox.Text;

        // Validate VPN Name
        if (string.IsNullOrWhiteSpace(name))
        {
            var dialog = new ContentDialog
            {
                Title = "Error",
                Content = "VPN name cannot be empty.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
            return;
        }

        if (configFile == null)
        {
            var dialog = new ContentDialog
            {
                Title = "Error",
                Content = "Please select a configuration file.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
            return;
        }

        // Save VPN Configuration to local storage
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        var randomName = Guid.NewGuid().ToString();
        StorageFile file = await localFolder.CreateFileAsync(randomName, CreationCollisionOption.ReplaceExisting);
        var vpnConfig = new VpnConfiguration
        {
            Type = "OpenVPN",
            Name = name,
            Configuration = configFile.Path,
            CreatedAt = DateTime.Now
        };

        string json = JsonSerializer.Serialize(vpnConfig);
        await FileIO.WriteTextAsync(file, json);

        // Navigate back to ShellPage and pass the new VPN
        Frame.Navigate(typeof(ShellPage), vpnConfig);
    }

}
