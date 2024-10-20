using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace multivpn.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AddVpnResultPage : Page
{
    public AddVpnResultPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        bool success = (bool)e.Parameter;
        ResultTextBlock.Text = success ? "VPN added successfully!" : "Failed to add VPN.";
    }

    private void OnGoBackClick(object sender, RoutedEventArgs e)
    {
        Frame.GoBack();
    }
}
