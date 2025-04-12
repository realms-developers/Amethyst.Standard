using Amethyst.Commands;
using Amethyst.Commands.Attributes;
using Amethyst.Players;

namespace Teleportation.Commands;

public static partial class Commands
{
    [ServerCommand(CommandType.Shared, "tppos", "teleportation.tppos.desc", "teleportation.tppos")]
    [CommandsSyntax("<x>", "<y>")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void TeleportPosition(CommandInvokeContext ctx, float x, float y)
    {
        NetPlayer from = ctx.Sender as NetPlayer ?? throw new InvalidCastException();

        from.Utils.Teleport(x, y);

        from.ReplySuccess("teleportation.tppos.success", x, y);
    }
}
