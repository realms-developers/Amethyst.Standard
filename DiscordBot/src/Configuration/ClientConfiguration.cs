using DSharpPlus;

namespace DiscordBot.Configuration;

internal class ClientConfiguration
{
    public string Token = string.Empty;
    public TokenType TokenType = TokenType.Bot;
    public DiscordIntents Intents = DiscordIntents.AllUnprivileged;
    public int ShardId = 0;
    public int ShardCount = 1;
}
