using Amethyst.Kernel;
using Amethyst.Storages.Config;
using DiscordBot.Configuration;
using DSharpPlus;
using DSharpPlus.Entities;
using static DiscordBot.DiscordBot;

namespace DiscordBot;

public class BotClient
{
    private readonly Configuration<ClientConfiguration> _clientcfg;
    private readonly Configuration<StatusConfiguration> _statuscfg;

    public DiscordClient Client { get; private set; }

    public bool IsConnected => Client.CurrentApplication != null;

    public BotClient(string name)
    {
        _clientcfg = new($"ClientConfiguration.{name}", new());
        _statuscfg = new($"StatusConfiguration.{name}", new());

        _clientcfg.Load();
        _statuscfg.Load();

        Microsoft.Extensions.Logging.LogLevel level = AmethystSession.Profile.DebugMode ?
            Microsoft.Extensions.Logging.LogLevel.Information :
            Microsoft.Extensions.Logging.LogLevel.None;

        Client = new(new()
        {
            Token = _clientcfg.Data.Token,
            TokenType = _clientcfg.Data.TokenType,
            Intents = _clientcfg.Data.Intents,
            ShardId = _clientcfg.Data.ShardId,
            ShardCount = _clientcfg.Data.ShardCount,
            MinimumLogLevel = level
        });

        GlobalClients.Add(this);
    }

    public void Connect()
    {
        DiscordActivity? status = string.Equals(_statuscfg.Data.StatusText, string.Empty, StringComparison.Ordinal) ?
            null : new(_statuscfg.Data.StatusText, _statuscfg.Data.ActivityType);

        Client.ConnectAsync(status, _statuscfg.Data.UserStatus).Wait();
    }

    public void Disconnect() => Client.DisconnectAsync().Wait();
}
