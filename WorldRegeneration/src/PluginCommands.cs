using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Dynamic.Attributes;
using Amethyst.Systems.Users.Base;

namespace WorldRegeneration;

public static class PluginCommands
{
    [Command("wregen save", "wregen.desc.saveworld")]
    [CommandPermission("wregen.save")]
    [CommandRepository("shared")]
    [CommandSyntax("en-US", "[name]")]
    [CommandSyntax("ru-RU", "[имя]")]
    public static void SaveWorld(IAmethystUser user, CommandInvokeContext ctx, string? name = null)
    {
        name ??= RegenUtils.GetDefaultWorldPath();

        RegenUtils.SaveWorld(name);
        ctx.Messages.ReplyInfo("wregen.reply.saved", name);
    }

    [Command("wregen load", "wregen.desc.loadworld")]
    [CommandPermission("wregen.load")]
    [CommandRepository("shared")]
    [CommandSyntax("en-US", "[name]")]
    [CommandSyntax("ru-RU", "[имя]")]
    public static void LoadWorld(IAmethystUser user, CommandInvokeContext ctx, string? name = null)
    {
        name ??= RegenUtils.GetDefaultWorldPath();

        RegenUtils.LoadWorld(name);
        ctx.Messages.ReplyInfo("wregen.reply.loaded", name);
        PluginMain.NextRegenerationTime = DateTime.UtcNow.AddMinutes(RegenConfiguration.Instance.AutoRegenerateMinutes);
    }
}