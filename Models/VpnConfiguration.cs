namespace multivpn.Models;

public class VpnConfiguration
{
    public string VpnType
    {
        get; set;
    } // The VPN type (Wireguard, OpenVPN, etc.)
    public string ConfigFilePath
    {
        get; set;
    } // Path to the VPN config file
    public string DisplayName
    {
        get; set;
    } // Friendly display name for the VPN
}

