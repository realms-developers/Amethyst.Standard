using DSharpPlus.Entities;

namespace DiscordBot.Configuration;

internal struct StatusConfiguration
{
    public StatusConfiguration() { }

    public UserStatus UserStatus = UserStatus.Online;
    public string StatusText = string.Empty;
    public ActivityType ActivityType = ActivityType.Playing;
}
