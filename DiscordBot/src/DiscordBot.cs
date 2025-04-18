using Amethyst.Extensions.Modules;
using Amethyst.Storages.Config;
using DiscordBot.Configuration;
using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordBot;

[AmethystModule(nameof(DiscordBot))]
public static class DiscordBot
{
    internal static bool _isInitialized;

    internal static readonly Configuration<ClientConfiguration> _clientcfg = new(typeof(ClientConfiguration).FullName!, new());
    internal static readonly Configuration<StatusConfiguration> _statuscfg = new(typeof(StatusConfiguration).FullName!, new());

    public static DiscordClient Client { get; internal set; } = null!;

    public static bool IsConnected => Client.CurrentApplication != null;

    [ModuleInitialize]
    public static void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        _clientcfg.Load();
        _statuscfg.Load();

        Client = new(new()
        {
            Token = _clientcfg.Data.Token,
            TokenType = _clientcfg.Data.TokenType,
            Intents = _clientcfg.Data.Intents,
            ShardId = _clientcfg.Data.ShardId,
            ShardCount = _clientcfg.Data.ShardCount
        });
    }

    public static void Connect()
    {
        DiscordActivity? status =
                    string.IsNullOrWhiteSpace(_statuscfg.Data.StatusText) ? null : new(_statuscfg.Data.StatusText, _statuscfg.Data.ActivityType);

        Client.ConnectAsync(status, _statuscfg.Data.UserStatus).Wait();
    }

    public static void Disconnect() => Client.DisconnectAsync().Wait();
}
