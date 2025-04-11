using System.Collections.Generic;
using Amethyst;
using Amethyst.Commands;
using Amethyst.Commands.Attributes;
using Amethyst.Core;
using Amethyst.Players;
using Amethyst.Text;
using Microsoft.Xna.Framework;

namespace Essentials.Commands;

public static class BasicCommands
{
    [ServerCommand(CommandType.Shared, "who", "essentials.desc.who", null)]
    [CommandsSyntax("[page]")]
    public static void Who(CommandInvokeContext ctx, int page = 0)
    {
        IEnumerable<NetPlayer> activePlayers = PlayerManager.Tracker
            .Where(p => p.IsActive);

        var pages = PagesCollection.CreateFromList(activePlayers
            .Select(p => p.Name));

        ctx.Sender.ReplyPage(pages, Localization.Get("essentials.header.who", ctx.Sender.Language),
            Localization.Get("essentials.footer.who", ctx.Sender.Language),
            [activePlayers.Count(), AmethystSession.Profile.MaxPlayers], false, page);
    }

    [ServerCommand(CommandType.Shared, "broadcast", "essentials.desc.broadcast", "essentials.broadcast")]
    [CommandsSyntax("<msg>")]
    public static void Broadcast(CommandInvokeContext _, string msg) => PlayerUtilities.BroadcastText(msg, Color.GhostWhite);
}
