using Amethyst.Extensions.Base.Metadata;
using Amethyst.Extensions.Modules;

namespace DiscordBot;

[ExtensionMetadata(nameof(DiscordBot), "realms-developers")]
public static class DiscordBot
{
    public static BotClient MainClient { get; } = new BotClient("Main");

    public static List<BotClient> GlobalClients { get; } = [];

    [ModuleInitialize]
    public static void Initialize() { }
}
