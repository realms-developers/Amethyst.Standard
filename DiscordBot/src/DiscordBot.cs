using Amethyst.Extensions.Base.Metadata;
using Amethyst.Extensions.Modules;

namespace DiscordBot;

[ExtensionMetadata(nameof(DiscordBot), "realms-developers")]
public static class DiscordBot
{
    static DiscordBot()
    {
        GlobalClients = [];

        MainClient = new("Main");
        MainClient.Connect();
    }

    public static BotClient MainClient { get; }

    public static List<BotClient> GlobalClients { get; }

    [ModuleInitialize]
    public static void Initialize() { }
}
