using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Dynamic.Attributes;
using Amethyst.Systems.Users.Base;

namespace WorldRegeneration;

public static class PluginCommands
{
    [Command("wregen save", "wregen.desc.saveworld")]
    [CommandPermission("wregen.save")]
    [CommandRepository("shared")]
    public static void SaveWorld(IAmethystUser user, CommandInvokeContext ctx)
    {
        
    }
}