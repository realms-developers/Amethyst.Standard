using Amethyst.Commands;
using Amethyst.Commands.Attributes;
using Amethyst.Text;
using WorldBans.Storages;

namespace WorldBans.Commands;

public static partial class Commands
{
    private const string _permission = "worldbans.projban";

    [ServerCommand(CommandType.Shared, "projban list", "worldbans.projban.list.desc", _permission)]
    [CommandsSyntax("[page]")]
    public static void ListProjectileBan(CommandInvokeContext ctx, int page = 0)
    {
        ProjectileBan[] bans = ProjectileBan.Collection;

        if (bans.Length == 0)
        {
            ctx.Sender.ReplyError("worldbans.projban.list.empty");

            return;
        }

        PagesCollection pages = PagesCollection.CreateFromList(bans.Select(b => b.ProjectileID.ToString()), 10);

        ctx.Sender.ReplyPage(pages, "worldbans.projban.list.header", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "projban add", "worldbans.projban.add.desc", _permission)]
    [CommandsSyntax("<proj_id>")]
    public static void AddProjectileBan(CommandInvokeContext ctx, ushort id)
    {
        ProjectileBan ban = new()
        {
            ProjectileID = id
        };

        ban.Save();

        ctx.Sender.ReplySuccess("worldbans.projban.add.success", id);
    }

    [ServerCommand(CommandType.Shared, "projban rm", "worldbans.projban.rm.desc", _permission)]
    [CommandsSyntax("<proj_id>")]
    public static void RemoveProjectileBan(CommandInvokeContext ctx, ushort id)
    {
        ProjectileBan ban = ProjectileBan.GetByProjectileId(id) ?? throw new NullReferenceException();

        ban.Delete();

        ctx.Sender.ReplySuccess("worldbans.projban.rm.success", id);
    }
}
