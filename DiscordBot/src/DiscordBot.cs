using Amethyst.Extensions.Modules;
using Amethyst.Storages.Config;
using DiscordBot.Configuration;
using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordBot;

[AmethystModule("DiscordBot",
    ["deps/Microsoft.Extensions.Logging.Abstractions.dll", "deps/DSharpPlus.dll"])]
public static class DiscordBot
{
    private static bool _isInitialized;

    private static readonly Configuration<ClientConfiguration> _clientcfg = new(typeof(ClientConfiguration).FullName!, new());
    private static readonly Configuration<StatusConfiguration> _statuscfg = new(typeof(StatusConfiguration).FullName!, new());

    public static DiscordClient Client { get; private set; } = null!;

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
