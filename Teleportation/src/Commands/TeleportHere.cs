using Amethyst.Commands;
using Amethyst.Commands.Arguments;
using Amethyst.Commands.Attributes;
using Amethyst.Players;

namespace Teleportation.Commands;

public static partial class Commands
{
    [ServerCommand(CommandType.Shared, "tphere", "teleportation.tphere.desc", "teleportation.tphere")]
    [CommandsSyntax("<to_player>")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void TeleportHere(CommandInvokeContext ctx, PlayerReference toRef)
    {
        NetPlayer from = ctx.Sender as NetPlayer ?? throw new InvalidCastException();

        NetPlayer to = toRef.Player;

        if (from.Name == to.Name)
        {
            throw new ArgumentException("teleportation.tprequest.same");
        }

        to.Utils.Teleport(from.Utils.PosX, from.Utils.PosY);

        from.ReplySuccess("teleportation.tphere.success.from", to.Name);

        if (!ctx.Sender.HasPermission("teleportation.silent"))
        {
            to.ReplyInfo("teleportation.tphere.success.to", from.Name);
        }
    }
}
