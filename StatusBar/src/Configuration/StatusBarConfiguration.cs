namespace StatusBar.Configuration;

internal class StatusBarConfiguration
{
    public string Header = string.Empty;
    public string Footer = string.Empty;
    public string Separator = "\n";
    public string[] Plugins = [];
    public long UpdateIntervalMs = 500;
    public bool Padding = false;
}
