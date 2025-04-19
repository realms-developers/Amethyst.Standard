using DSharpPlus.Entities;

namespace DiscordBot.Configuration;

internal struct StatusConfiguration
{
    public StatusConfiguration() { }

    public UserStatus UserStatus = UserStatus.Online;
    public ActivityType ActivityType = ActivityType.Playing;
    public string StatusText = string.Empty;
}
