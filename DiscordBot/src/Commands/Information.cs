using Amethyst.Commands;
using Amethyst.Commands.Attributes;
using static DiscordBot.DiscordBot;

namespace DiscordBot.Commands;

public static partial class Commands
{
    [ServerCommand(CommandType.Console, "discord-info", "Displays information about the Discord bot.", null)]
    public static void Information(CommandInvokeContext ctx)
    {
        if (Client.CurrentApplication == null)
        {
            ctx.Sender.ReplyError("Bot isn't initialized!");

            return;
        }

        ctx.Sender.ReplyInfo($"{Client.CurrentUser.Username}:");
        ctx.Sender.ReplyInfo($" -  {nameof(Client.CurrentUser.Presence.Status)}: {Client.CurrentUser.Presence.Status}");
        ctx.Sender.ReplyInfo($" -  {nameof(Client.CurrentUser.Id)}: {Client.CurrentUser.Id}");
        ctx.Sender.ReplyInfo($" -  {nameof(Client.CurrentUser.IsBot)}: {Client.CurrentUser.IsBot}");
        ctx.Sender.ReplyInfo($" -  {nameof(Client.Ping)}: {Client.Ping}");
        ctx.Sender.ReplyInfo($" -  {nameof(Client.ShardCount)}: {Client.ShardCount}");
        ctx.Sender.ReplyInfo($" -  {nameof(Client.ShardId)}: {Client.ShardId}");
    }
}
