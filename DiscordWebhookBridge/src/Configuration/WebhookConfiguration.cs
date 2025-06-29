namespace DiscordWebhookBridge.Configuration;

internal class WebhookConfiguration
{
    public string[] WebhookUri = [];
    // WE ARE DONT SUPPORT UUID DISPLAYING DSAF!
    public string ContentFormat = "{0}"; // 0 for content, 1 for name, 2 for IP, 3 for Index
    public string AvatarUrl = string.Empty;
    public bool AllowPinging = false;
}
