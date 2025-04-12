using Amethyst.Commands;
using Amethyst.Commands.Arguments;
using Amethyst.Commands.Attributes;
using Amethyst.Players;

namespace Teleportation.Commands;

public static partial class Commands
{
    [ServerCommand(CommandType.Shared, "tpaccept", "teleportation.tpaccept.desc", null)]
    [CommandsSyntax("[from_player]")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void TeleportAccept(CommandInvokeContext ctx, PlayerReference? toRef = null)
    {
        NetPlayer to = ctx.Sender as NetPlayer ?? throw new InvalidCastException();

        NetPlayer? from = toRef?.Player;

        Request? request = Request.GlobalRequests
            .FirstOrDefault(r => r.IsActive && (from == null ? r.To == to : r.From == from && r.To == to)) ??
            throw new NullReferenceException("teleportation.notfound");

        request.Efectuate();

        if (!request.From!.HasPermission("teleportation.silent"))
        {
            to.ReplySuccess("teleportation.tpaccept.success.to", request.From!.Name);
        }

        request.From!.ReplySuccess("teleportation.tpaccept.success.from", to.Name);
    }
}
