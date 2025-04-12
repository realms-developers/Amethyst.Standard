using Amethyst.Commands;
using Amethyst.Commands.Arguments;
using Amethyst.Commands.Attributes;
using Amethyst.Players;

namespace Teleportation.Commands;

public static partial class Commands
{
    [ServerCommand(CommandType.Shared, "tpdeny", "teleportation.tpdeny.desc", null)]
    [CommandsSyntax("[from_player]")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void TeleportDeny(CommandInvokeContext ctx, PlayerReference? toRef = null)
    {
        NetPlayer to = ctx.Sender as NetPlayer ?? throw new InvalidCastException();

        NetPlayer? from = toRef?.Player;

        Request? request = Request.GlobalRequests
            .FirstOrDefault(r => r.IsActive && (from == null ? r.To == to : r.From == from && r.To == to)) ??
            throw new NullReferenceException("teleportation.notfound");

        to.ReplySuccess("teleportation.tpdeny.success.to", request.From!.Name);

        if (!ctx.Sender.HasPermission("teleportation.silent"))
        {
            request.From!.ReplyWarning("teleportation.tpdeny.success.from", to.Name);
        }

        request.Remove();
    }
}
