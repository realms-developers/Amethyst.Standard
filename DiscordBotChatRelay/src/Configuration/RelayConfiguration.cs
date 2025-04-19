namespace DiscordBotChatRelay.Configuration;

internal struct RelayConfiguration
{
    public RelayConfiguration() { }

    public ulong[] ChannelIds = [];
    public string Format = "Discord> <{1}> {0}";
}
