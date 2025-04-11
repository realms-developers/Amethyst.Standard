using Amethyst;
using Amethyst.Commands;
using Amethyst.Commands.Attributes;
using Amethyst.Players;
using Amethyst.Text;

namespace Essentials.Commands;

public static class BasicCommands
{
    [ServerCommand(CommandType.Shared, "who", "essentials.desc.who", null)]
    [CommandsSyntax("[page]")]
    public static void Who(CommandInvokeContext ctx, int page = 0)
    {
        var pages = PagesCollection.CreateFromList(PlayerManager.Tracker.Where(p => p.IsActive).Select(p => p.Name));
        ctx.Sender.ReplyPage(pages, Localization.Get("essentials.header.who", ctx.Sender.Language), null, null, false, page);
    }
}
