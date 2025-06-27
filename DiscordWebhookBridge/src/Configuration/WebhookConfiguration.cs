namespace DiscordWebhookBridge.Configuration;

internal class WebhookConfiguration
{
    public string[] WebhookUri = [];
    public string ContentFormat = "{0}"; // 0 for content, 1 for name, 2 for UUID, 3 for IP, 4 for Index
    public string AvatarUrl = string.Empty;
    public bool AllowPinging = false;
}
