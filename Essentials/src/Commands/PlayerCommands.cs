﻿using Amethyst.Commands;
using Amethyst.Commands.Arguments;
using Amethyst.Commands.Attributes;
using Amethyst.Players;
using Amethyst.Text;

namespace Essentials.Commands;

public static class PlayerCommands
{
    [ServerCommand(CommandType.Shared, "slap", "essentials.desc.slap", "essentials.slap")]
    [CommandsSyntax("<player>", "<damage>")]
    public static void Slap(CommandInvokeContext ctx, PlayerReference toRef, ushort damage)
    {
        NetPlayer to = toRef.Player;

        to.Utils.Hurt(damage);

        ctx.Sender.ReplySuccess("essentials.text.slap", to.Name, damage);
    }

    [ServerCommand(CommandType.Shared, "kill", "essentials.desc.kill", "essentials.kill")]
    [CommandsSyntax("<player>")]
    public static void Kill(CommandInvokeContext ctx, PlayerReference toRef)
    {
        NetPlayer to = toRef.Player;

        to.Utils.Kill();

        ctx.Sender.ReplySuccess("essentials.text.kill", to.Name);
    }

    [ServerCommand(CommandType.Shared, "buff", "essentials.desc.buff", "essentials.buff")]
    [CommandsSyntax("<buff_id>", "[time]")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void Buff(CommandInvokeContext ctx, int id, string time = "1m")
    {
        NetPlayer from = ctx.Sender as NetPlayer ?? throw new InvalidCastException();

        int seconds = TextUtility.ParseToSeconds(time);

        from.Utils.AddBuff(id, TimeSpan.FromSeconds(seconds));

        from.ReplySuccess("essentials.text.buff", id);
    }

    [ServerCommand(CommandType.Shared, "gbuff", "essentials.desc.gbuff", "essentials.gbuff")]
    [CommandsSyntax("<player>", "<buff_id>", "[time]")]
    public static void GiveBuff(CommandInvokeContext ctx, PlayerReference toRef, int id, string time = "1m")
    {
        int seconds = TextUtility.ParseToSeconds(time);

        toRef.Player.Utils.AddBuff(id, TimeSpan.FromSeconds(seconds));

        ctx.Sender.ReplySuccess("essentials.text.buff", id);
    }

    [ServerCommand(CommandType.Shared, "spawn", "essentials.desc.spawn", "essentials.spawn")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void Spawn(CommandInvokeContext ctx)
    {
        NetPlayer from = ctx.Sender as NetPlayer ?? throw new InvalidCastException();

        float x = from.TPlayer.SpawnX == -1 ? Terraria.Main.spawnTileX : from.TPlayer.SpawnX;
        float y = from.TPlayer.SpawnY == -1 ? Terraria.Main.spawnTileY : from.TPlayer.SpawnY;

        from.Utils.Teleport(x * 16, y * 16 - 48);

        from.ReplySuccess("essentials.text.spawn");
    }

    [ServerCommand(CommandType.Shared, "pos", "essentials.desc.pos", null)]
    [CommandsSyntax("[player]")]
    public static void Position(CommandInvokeContext ctx, PlayerReference? toRef = null)
    {
        NetPlayer to = toRef == null ? ctx.Sender as NetPlayer ?? throw new InvalidCastException() : toRef.Player;

        if (to.Name != ctx.Sender.Name && !ctx.Sender.HasPermission("essentials.pos"))
        {
            ctx.Sender.ReplyError("commands.noPermission");

            return;
        }

        ctx.Sender.ReplySuccess("essentials.text.pos", to.Utils.PosX, to.Utils.PosY);
    }

    [ServerCommand(CommandType.Shared, "heal", "essentials.desc.heal", "essentials.heal")]
    [CommandsSyntax("[amount]", "[player]")]
    public static void Heal(CommandInvokeContext ctx, ushort amount = ushort.MaxValue, PlayerReference? toRef = null)
    {
        NetPlayer to = toRef == null ? ctx.Sender as NetPlayer ?? throw new InvalidCastException() : toRef.Player;

        bool other = to.Name != ctx.Sender.Name;

        if (other)
        {
            if (!ctx.Sender.HasPermission("essentials.heal.others"))
            {
                ctx.Sender.ReplyError("commands.noPermission");

                return;
            }

            ctx.Sender.ReplySuccess("essentials.text.heal.other", to.Name, amount);
        }

        int heal = Math.Min(amount, to.TPlayer.statLifeMax2 - to.TPlayer.statLife);

        to.Utils.Heal(heal);

        to.ReplySuccess("essentials.text.heal", heal);
    }
}
