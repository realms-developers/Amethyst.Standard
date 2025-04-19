using Amethyst.Extensions.Modules;

namespace DiscordBot;

[AmethystModule(nameof(DiscordBot))]
public static class DiscordBot
{
    public static List<BotClient> GlobalClients { get; } = [];

    [ModuleInitialize]
    public static void Initialize() { }
}
