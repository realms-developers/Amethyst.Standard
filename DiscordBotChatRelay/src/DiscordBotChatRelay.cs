using Amethyst;
using Amethyst.Extensions.Base.Metadata;
using Amethyst.Extensions.Plugins;
using Amethyst.Network.Structures;
using Amethyst.Server.Entities.Players;
using Amethyst.Storages.Config;
using DiscordBot;
using DiscordBotChatRelay.Configuration;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DiscordBotChatRelay;

[ExtensionMetadata(nameof(DiscordBotChatRelay), "realms-developers")]
public sealed class DiscordBotChatRelay : PluginInstance
{
    private static readonly Configuration<RelayConfiguration> _relayCfg = new(nameof(RelayConfiguration), new());
    private static readonly BotClient _client = new(nameof(DiscordBotChatRelay));
    private static readonly NetColor _color = new(255, 255, 255);

    protected override void Load()
    {
        _relayCfg.Load();

        _client.Client.MessageCreated += OnMessageCreated;

        _client.Connect();
    }

    protected override void Unload() => _client.Disconnect();

    private static Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs args)
    {
        DiscordMessage msg = args.Message;
        DiscordUser author = msg.Author;

        if (_relayCfg.Data.ChannelIds.Any(i => msg.ChannelId == i) && !author.IsBot)
        {
            AmethystLog.System.Info("DiscordChatRelay", $"$!b#{args.Channel.Name} $!r$b@$!d{author.Username}$!r: $!d{msg.Content}");
            PlayerUtils.BroadcastText(_relayCfg.Data.Format, _color,
                msg.Content, author.Username, author.Id, args.Channel.Name, msg.ChannelId);
        }

        return Task.CompletedTask;
    }
}
