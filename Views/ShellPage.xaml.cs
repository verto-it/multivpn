using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using multivpn.Contracts.Services;
using multivpn.Helpers;
using multivpn.Models;
using multivpn.ViewModels;
using Windows.Storage;
using Windows.System;

namespace multivpn.Views;

// TODO: Update NavigationViewItem titles and icons in ShellPage.xaml.

public class VpnConfiguration
{
    public string Name
    {
        get; set;
    }
    public string Type
    {
        get; set;
    }
    public string Configuration
    {
        get; set;
    }
    public DateTime CreatedAt
    {
        get; set;
    }
}

public sealed partial class ShellPage : Page
{
    public ShellViewModel ViewModel
    {
        get;
    }

    public ShellPage(ShellViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        ViewModel.NavigationService.Frame = NavigationFrame;
        ViewModel.NavigationViewService.Initialize(NavigationViewControl);

        // TODO: Set the title bar icon by updating /Assets/WindowIcon.ico.
        // A custom title bar is required for full window theme and Mica support.
        // https://docs.microsoft.com/windows/apps/develop/title-bar?tabs=winui3#full-customization
        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.MainWindow.Activated += MainWindow_Activated;
        AppTitleBarText.Text = "AppDisplayName".GetLocalized();

        // Load saved VPN configurations
        LoadVpnConfigurations();
    }

    private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        TitleBarHelper.UpdateTitleBar(RequestedTheme);

        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        App.AppTitlebar = AppTitleBarText as UIElement;
    }

    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        AppTitleBar.Margin = new Thickness()
        {
            Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
            Top = AppTitleBar.Margin.Top,
            Right = AppTitleBar.Margin.Right,
            Bottom = AppTitleBar.Margin.Bottom
        };
    }

    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

        if (modifiers.HasValue)
        {
            keyboardAccelerator.Modifiers = modifiers.Value;
        }

        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

        return keyboardAccelerator;
    }

    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        var navigationService = App.GetService<INavigationService>();

        var result = navigationService.GoBack();

        args.Handled = result;
    }

    private void OnAddVpnTapped(object sender, TappedRoutedEventArgs e)
    {
        // Navigate to AddVPNPage when "Add VPN Connection" is clicked
        NavigationFrame.Navigate(typeof(AddVPNPage));
    }

    private void AddVpnToNavigationView(string vpnName)
    {
        var newItem = new NavigationViewItem
        {
            Content = vpnName,
            Icon = new FontIcon { Glyph = "\uE77B" }
        };

        // Handle the item click event to navigate to the VPN details page
        newItem.Tapped += (sender, args) =>
        {
            string[] vpnDetails = new string[] { vpnName, "WireGuard" }; // Example VPN type

            // Navigate to a page showing VPN details (connect, edit, delete options)
            NavigationFrame.Navigate(typeof(VpnDetailPage), vpnDetails);
        };

        // Add the new VPN item to the NavigationView
        NavigationViewControl.MenuItems.Add(newItem);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        // Check if a VPN configuration is passed when navigating back from AddVPNPage
        if (e.Parameter is VpnConfiguration vpnConfig)
        {
            // Add the new VPN to the navigation view
            AddVpnToNavigationView(vpnConfig.Name);
        }

        // Load saved VPN configurations
        LoadVpnConfigurations();
    }

    // Method to load VPN configurations from local storage
    private async Task LoadVpnConfigurations()
    {
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        IReadOnlyList<StorageFile> files = await localFolder.GetFilesAsync();

        files = files.OrderByDescending(f => f.DateCreated).ToList();

        foreach (StorageFile file in files) {
            string json = await FileIO.ReadTextAsync(file);
            var vpnConfig = JsonSerializer.Deserialize<VpnConfiguration>(json);
            AddVpnToNavigationView(vpnConfig.Name);
        }

    }
}
