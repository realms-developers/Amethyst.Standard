using Amethyst.Core;
using Amethyst.Extensions.Plugins;
using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Amethyst.Storages.Config;
using DiscordWebhookBridge.Configuration;
using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordWebhookBridge;

public sealed class DiscordWebhookBridge : PluginInstance
{
    internal static readonly Configuration<WebhookConfiguration> _webhookcfg = new(typeof(WebhookConfiguration).FullName!, new());

    internal static DiscordWebhook[] _registered = null!;
    internal static DiscordWebhookClient _client = new();

    public override string Name => nameof(DiscordWebhookBridge);

    public override Version Version => new(1, 0);

    protected override void Load()
    {
        _webhookcfg.Load();

        string[] webhookUri = _webhookcfg.Data.WebhookUri;

        if (webhookUri.Length == 0)
        {
            AmethystLog.Main.Error(Name, "WebhookUri is empty. Loading canceled.");

            return;
        }

        _registered = new DiscordWebhook[webhookUri.Length];

        for (int i = 0; i < webhookUri.Length; i++)
        {
            _registered[i] = _client.AddWebhookAsync(new(webhookUri[i])).Result;

            AmethystLog.Main.Info(Name, $"Loaded webhook {i}.");
        }

        NetworkManager.Binding.AddInModule(ModuleTypes.NetText, OnPlayerText);
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

        NetworkManager.Binding.RemoveInModule(ModuleTypes.NetText, OnPlayerText);
    }

    internal static void OnPlayerText(in IncomingModule packet, PacketHandleResult result)
    {
        BinaryReader reader = packet.GetReader();

        string command = reader.ReadString();
        string text = reader.ReadString();

        if (command != "Say" || text.StartsWith(Amethyst.Commands.CommandsManager.CommandPrefix))
        {
            return;
        }

        string content = _webhookcfg.Data.AllowPinging ? text : text.Replace("@", "@\u200B");

        DiscordWebhookBuilder builder = new DiscordWebhookBuilder()
            .WithUsername(packet.Player.Name)
            .WithContent(content);

        _client.BroadcastMessageAsync(builder).Wait();
    }
}
