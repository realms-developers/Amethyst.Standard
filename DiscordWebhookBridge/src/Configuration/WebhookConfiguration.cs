namespace DiscordWebhookBridge.Configuration;

internal class WebhookConfiguration
{
    public string[] WebhookUri = [];
    public string ContentFormat = "{anvil.chat.customprefix|anvil.perms.prefix} {realname} {anvil.perms.suffix}: {modifiedtext|realtext}";
    public string UserFormat = "{anvil.chat.customprefix|anvil.perms.prefix} {realname}";
    public Dictionary<string, string> PermissionedAvatars = new Dictionary<string, string>();
    public string DefaultAvatarUrl = string.Empty;
    public bool AllowPinging = false;
}
