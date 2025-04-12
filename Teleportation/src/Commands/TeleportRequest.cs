using Amethyst.Commands;
using Amethyst.Commands.Arguments;
using Amethyst.Commands.Attributes;
using Amethyst.Players;

namespace Teleportation.Commands;

public static partial class Commands
{
    [ServerCommand(CommandType.Shared, "tprequest", "teleportation.tprequest.desc", "teleportation.tprequest")]
    [CommandsSyntax("<to_player>")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void TeleportRequest(CommandInvokeContext ctx, PlayerReference toRef)
    {
        NetPlayer? from = ctx.Sender as NetPlayer ?? throw new InvalidCastException();

        NetPlayer to = toRef.Player;

        if (from.Name == to.Name)
        {
            throw new ArgumentException("teleportation.tprequest.same");
        }

        _ = new Request(from, to);

        from.ReplySuccess("teleportation.tprequest.success.from", to.Name);

        to.ReplyInfo("teleportation.tprequest.success.to", from.Name);
    }

    [ServerCommand(CommandType.Shared, "tpr", "teleportation.tprequest.desc", "teleportation.tprequest")]
    [CommandsSyntax("<to_player>")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void TeleportRequestAlias(CommandInvokeContext ctx, PlayerReference toRef) => TeleportRequest(ctx, toRef);
}
