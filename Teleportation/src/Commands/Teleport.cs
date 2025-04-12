using Amethyst.Commands;
using Amethyst.Commands.Arguments;
using Amethyst.Commands.Attributes;
using Amethyst.Players;

namespace Teleportation.Commands;

public static partial class Commands
{
    [ServerCommand(CommandType.Shared, "tp", "teleportation.tp.desc", "teleportation.tp")]
    [CommandsSyntax("<to_player>")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void Teleport(CommandInvokeContext ctx, PlayerReference toRef)
    {
        NetPlayer from = ctx.Sender as NetPlayer ?? throw new InvalidCastException();

        NetPlayer to = toRef.Player;

        if (from.Name == to.Name)
        {
            throw new ArgumentException("teleportation.tprequest.same");
        }

        from.Utils.Teleport(to.Utils.PosX, to.Utils.PosY);

        from.ReplySuccess("teleportation.tp.success.from", to.Name);

        if (!ctx.Sender.HasPermission("teleportation.silent"))
        {
            to.ReplyInfo("teleportation.tp.success.to", from.Name);
        }
    }
}
