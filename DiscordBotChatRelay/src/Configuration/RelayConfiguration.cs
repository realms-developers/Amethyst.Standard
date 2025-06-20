namespace DiscordBotChatRelay.Configuration;

internal class RelayConfiguration
{
    public ulong[] ChannelIds = [];
    public string Format = "Discord> <{1}> {0}";
}
