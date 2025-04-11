using Amethyst.Commands;
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
}
