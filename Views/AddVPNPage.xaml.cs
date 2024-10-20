using System.Diagnostics;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using multivpn.Models;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop; // For WindowNative

namespace multivpn.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AddVPNPage : Page
{
    private StorageFile configFile;

    public AddVPNPage()
    {
        this.InitializeComponent();
    }

    // Handle file selection
    private async void OnSelectFileClick(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".conf");
        picker.FileTypeFilter.Add(".ovpn");
        picker.SuggestedStartLocation = PickerLocationId.Desktop;

        // Fix: Get the window handle and initialize the picker with the window
        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        configFile = await picker.PickSingleFileAsync();
        if (configFile != null)
        {
            (sender as Button).Content = configFile.Name;
        }
    }

    // Handle form submission
    private async void OnSubmitVpn(object sender, RoutedEventArgs e)
    {
        if (configFile == null || VpnTypeComboBox.SelectedItem == null)
        {
            // Show error message if the form is incomplete
            ContentDialog dialog = new ContentDialog
            {
                Title = "Error",
                Content = "Please select a VPN type and upload a configuration file.",
                CloseButtonText = "OK"
            };
            _ = dialog.ShowAsync();
        }
        else
        {
            string vpnType = ((ComboBoxItem)VpnTypeComboBox.SelectedItem).Content.ToString();
            string displayName = vpnType + " Connection"; // You can change this to a custom name if needed

            // Create the VPN configuration object
            var vpnConfig = new VpnConfiguration
            {
                VpnType = vpnType,
                ConfigFilePath = configFile.Path,
                DisplayName = displayName
            };

            // Get the local folder for the app
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            // Save the VPN configuration in a JSON file
            string vpnConfigFileName = "VpnConfigurations.json";
            StorageFile vpnConfigFile = await localFolder.CreateFileAsync(vpnConfigFileName, CreationCollisionOption.OpenIfExists);

            // Read existing configurations, if any
            string existingData = await FileIO.ReadTextAsync(vpnConfigFile);
            var vpnConfigs = string.IsNullOrEmpty(existingData) ? new List<VpnConfiguration>() : JsonSerializer.Deserialize<List<VpnConfiguration>>(existingData);

            // Add the new VPN config to the list
            vpnConfigs.Add(vpnConfig);

            // Write the updated list back to the file
            string json = JsonSerializer.Serialize(vpnConfigs);
            await FileIO.WriteTextAsync(vpnConfigFile, json);

            // Überprüfen Sie, ob Frame und vpnConfig.DisplayName nicht null sind
            if (Frame != null && vpnConfig.DisplayName != null)
            {
                Frame.Navigate(typeof(AddVpnResultPage), true);
            }
            else
            {
                // Debugging-Ausgabe, um festzustellen, welches Objekt null ist
                if (Frame == null)
                {
                    Debug.WriteLine("Frame ist null.");
                }
                if (vpnConfig.DisplayName == null)
                {
                    Debug.WriteLine("vpnConfig.DisplayName ist null.");
                }

                // Zeigen Sie eine Fehlermeldung an, wenn eines der Objekte null ist
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Fehler",
                    Content = "Ein unerwarteter Fehler ist aufgetreten. Bitte versuchen Sie es erneut.",
                    CloseButtonText = "OK"
                };
                _ = dialog.ShowAsync();
            }



        }
    }

}
