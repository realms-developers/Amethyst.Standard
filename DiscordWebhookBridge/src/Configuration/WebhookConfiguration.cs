namespace DiscordWebhookBridge.Configuration;

internal struct WebhookConfiguration
{
    public WebhookConfiguration() { }

    public string[] WebhookUri = [];
    public bool AllowPinging = false;
}
