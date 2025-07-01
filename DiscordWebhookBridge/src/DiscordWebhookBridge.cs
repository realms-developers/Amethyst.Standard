using Amethyst;
using Amethyst.Extensions.Base.Metadata;
using Amethyst.Extensions.Plugins;
using Amethyst.Storages.Config;
using Amethyst.Systems.Chat;
using DiscordWebhookBridge.Configuration;
using DiscordWebhookBridge.Handling;
using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordWebhookBridge;

[ExtensionMetadata(nameof(DiscordWebhookBridge), "realms-developers")]
public sealed class DiscordWebhookBridge : PluginInstance
{
    private static readonly DiscordChatOutput _chatOutput = new();

    private static DiscordWebhook[] _registered = null!;

    internal static readonly Configuration<WebhookConfiguration> _webhookcfg = new(nameof(WebhookConfiguration), new());

    internal static DiscordWebhookClient _client = new();

    protected override void Load()
    {
        _webhookcfg.Load();

        string[] webhookUri = _webhookcfg.Data.WebhookUri;

        if (webhookUri.Length == 0)
        {
            AmethystLog.Main.Error(nameof(DiscordWebhookBridge), "WebhookUri is empty. Loading canceled.");

            return;
        }

        _registered = new DiscordWebhook[webhookUri.Length];

        for (int i = 0; i < webhookUri.Length; i++)
        {
            _registered[i] = _client.AddWebhookAsync(new(webhookUri[i])).Result;

            AmethystLog.Main.Info(nameof(DiscordWebhookBridge), $"Loaded webhook {i}.");
        }

        ServerChat.OutputRegistry.Add(_chatOutput);
    }

    protected override void Unload()
    {
        if (_registered == null)
        {
            return;
        }

        foreach (DiscordWebhook webhook in _registered)
        {
            _client.RemoveWebhook(webhook.Id);
        }

        ServerChat.OutputRegistry.Remove(_chatOutput);
    }
}
