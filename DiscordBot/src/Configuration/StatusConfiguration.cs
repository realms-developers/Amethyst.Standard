using DSharpPlus.Entities;

namespace DiscordBot.Configuration;

internal class StatusConfiguration
{
    public UserStatus UserStatus = UserStatus.Online;
    public ActivityType ActivityType = ActivityType.Playing;
    public string StatusText = string.Empty;
}
