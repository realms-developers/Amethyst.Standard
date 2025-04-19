using Amethyst.Extensions.Plugins;
using Amethyst.Players;
using Amethyst.Storages.Config;
using DiscordBot;
using DiscordBotChatRelay.Configuration;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Xna.Framework;

namespace DiscordBotChatRelay;

public sealed class DiscordBotChatRelay : PluginInstance
{
    internal static readonly Configuration<RelayConfiguration> _relayCfg = new(typeof(RelayConfiguration).FullName!, new());
    internal static readonly BotClient _client = new(nameof(DiscordBotChatRelay));

    public override string Name => nameof(DiscordBotChatRelay);

    public override Version Version => new(1, 0);

    protected override void Load()
    {
        _relayCfg.Load();

        _client.Client.MessageCreated += OnMessageCreated;

        _client.Connect();
    }

    protected override void Unload() => _client.Disconnect();

    internal static Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs args)
    {
        DiscordMessage msg = args.Message;
        DiscordUser author = msg.Author;

        if (_relayCfg.Data.ChannelIds.Any(i => msg.ChannelId == i) && !author.IsBot)
        {
            PlayerUtilities.BroadcastText(
                string.Format(_relayCfg.Data.Format, msg.Content, author.Username, author.Id, args.Channel.Name, msg.ChannelId), Color.GhostWhite);
        }

        return Task.CompletedTask;
    }
}
