namespace DiscordWebhookBridge.Configuration;

internal class WebhookConfiguration
{
    public string[] WebhookUri = [];
    public string ContentFormat = "{anvil.chat.customprefix|anvil.perms.prefix} {realname} {anvil.perms.suffix}: {modifiedtext|realtext}";
    public string AvatarUrl = string.Empty;
    public bool AllowPinging = false;
}
