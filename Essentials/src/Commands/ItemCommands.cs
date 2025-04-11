using Amethyst;
using Amethyst.Commands;
using Amethyst.Commands.Arguments;
using Amethyst.Commands.Attributes;
using Amethyst.Players;

namespace Essentials.Commands;

public static class ItemCommands
{
    [ServerCommand(CommandType.Shared, "i", "essentials.desc.item", "essentials.items.item")]
    [CommandsSyntax("<$held | item name | item ID>", "[count]", "[prefix ID]")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void Item(CommandInvokeContext ctx, ItemReference itemRef, int stack = 9999, byte prefix = 1)
    {
        var plr = ctx.Sender as NetPlayer;

        plr!.Utils.GiveItem(itemRef.NetItem with { Stack = (short)Math.Clamp(stack, 1, itemRef.TItem.maxStack), Prefix = prefix });

        ctx.Sender.ReplySuccess(Localization.Get("essentials.text.itemCreated", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "give", "essentials.desc.giveItem", "essentials.items.give")]
    [CommandsSyntax("<player>", "<$held | item name | item ID>", "[count]", "[prefix ID]")]
    public static void GiveItem(CommandInvokeContext ctx, PlayerReference plrRef, ItemReference itemRef, int stack = 9999, byte prefix = 1)
    {
        plrRef.Player.Utils.GiveItem(itemRef.NetItem with { Stack = (short)Math.Clamp(stack, 1, itemRef.TItem.maxStack), Prefix = prefix });

        ctx.Sender.ReplySuccess(Localization.Get("essentials.text.itemCreated", ctx.Sender.Language));
    }

    // fills all items in inventory to max stack
    [ServerCommand(CommandType.Shared, "fill", "essentials.desc.fill", "essentials.items.fill")]
    [CommandsSyntax("[player]")]
    public static void Fill(CommandInvokeContext ctx, PlayerReference? plrRef = null)
    {
        NetPlayer? plr = plrRef?.Player ?? (ctx.Sender is NetPlayer ? ctx.Sender as NetPlayer : null);

        if (plr == null)
        {
            ctx.Sender.ReplyError(Localization.Get("essentials.text.noTargetPlayer", ctx.Sender.Language));
            return;
        }

        for (int i = 0; i < 59; i++)
        {
            Terraria.Item item = plr.TPlayer.inventory[i];
            if (item.netID > 0 && item.stack < item.maxStack)
            {
                plr.Utils.GiveItem(item.netID, item.maxStack - item.stack, item.prefix);
            }
        }

        ctx.Sender.ReplySuccess(Localization.Get("essentials.text.filled", ctx.Sender.Language));
    }
}
