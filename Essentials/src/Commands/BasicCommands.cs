using Amethyst.Commands;
using Amethyst.Players;
using Amethyst.Text;

namespace Amethyst.Essentials.Commands;

public static class BasicCommands
{
    [ServerCommand(CommandType.Shared, "who", "essentials.desc.who", null)]
    [CommandsSyntax("[page]")]
    public static void WhoCommand(CommandInvokeContext ctx, int pageId = 0)
    {
        var page = PagesCollection.CreateFromList(PlayerManager.Tracker.Where(p => p.IsActive).Select(p => p.Name));
        ctx.Sender.ReplyPage(page, Localization.Get("essentials.header.who", ctx.Sender.Language), null, null, false, pageId);
    }
}