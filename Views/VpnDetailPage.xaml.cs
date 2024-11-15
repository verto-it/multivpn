using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace multivpn.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class VpnDetailPage : Page
{
    private string vpnName;
    private string vpnType;

    public VpnDetailPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        var vpnDetails = e.Parameter as string[];
        vpnName = vpnDetails[0];
        vpnType = vpnDetails[1];
        VpnNameTextBlock.Text = vpnName;
    }

    private void OnConnectClick(object sender, RoutedEventArgs e)
    {
        // Handle VPN connection logic here

        // If the VPN Type is WireGuard, we need to do the following:
        // 1. Get The Path to the WireGuard Configuration File
        // 2. Get the WireGuard Installation Path
        // 3. Run WG with the command: "path" /installtunnelservice "configpath"
        // 4. Hide Connect Button and Show Disconnect Button
        // 5. Show Is Connected Indicator

        // If the VPN Type is OpenVPN, we need to do the following:
        // 1. Get The Path to the OpenVPN Configuration File
        // 2. Get the OpenVPN Installation Path
        // 3. Run OpenVPN with the command: "path" --config "configpath"
        // 4. Hide Connect Button and Show Disconnect Button
        // 5. Show Is Connected Indicator

        if (vpnName != null) {
            ConnectButton.Visibility = Visibility.Collapsed;
            DisconnectButton.Visibility = Visibility.Visible;
            IsConnectedIndicator.Visibility = Visibility.Visible;
        }



    }

    private void OnDisconnectClick(object sender, RoutedEventArgs e)
    {
        ConnectButton.Visibility = Visibility.Visible;
        DisconnectButton.Visibility = Visibility.Collapsed;
        IsConnectedIndicator.Visibility = Visibility.Collapsed;
    }

    private void OnEditClick(object sender, RoutedEventArgs e)
    {
        // Handle VPN edit logic here
    }

    private void OnDeleteClick(object sender, RoutedEventArgs e)
    {

    }
}
