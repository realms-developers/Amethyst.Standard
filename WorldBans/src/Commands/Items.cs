using Amethyst.Commands;
using Amethyst.Commands.Attributes;
using Amethyst.Text;
using WorldBans.Storages;

namespace WorldBans.Commands;

public static partial class Commands
{
    private const string _itemPermission = "worldbans.itemban";

    [ServerCommand(CommandType.Shared, "itemban list", "worldbans.itemban.list.desc", _itemPermission)]
    [CommandsSyntax("[page]")]
    public static void ListItemBan(CommandInvokeContext ctx, int page = 0)
    {
        ItemBan[] bans = ItemBan.Collection;

        if (bans.Length == 0)
        {
            ctx.Sender.ReplyError("worldbans.itemban.list.empty");

            return;
        }

        PagesCollection pages = PagesCollection.CreateFromList(bans.Select(b => b.ItemID.ToString()), 10);

        ctx.Sender.ReplyPage(pages, "worldbans.itemban.list.header", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "itemban add", "worldbans.itemban.add.desc", _itemPermission)]
    [CommandsSyntax("<item_id>")]
    public static void AddItemBan(CommandInvokeContext ctx, ushort id)
    {
        ItemBan ban = new()
        {
            ItemID = id
        };

        ban.Save();

        ctx.Sender.ReplySuccess("worldbans.itemban.add.success", id);
    }

    [ServerCommand(CommandType.Shared, "itemban rm", "worldbans.itemban.rm.desc", _itemPermission)]
    [CommandsSyntax("<item_id>")]
    public static void RemoveItemBan(CommandInvokeContext ctx, ushort id)
    {
        ItemBan ban = ItemBan.GetByItemId(id) ?? throw new NullReferenceException();

        ban.Delete();

        ctx.Sender.ReplySuccess("worldbans.itemban.rm.success", id);
    }
}
