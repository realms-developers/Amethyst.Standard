using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Dynamic.Attributes;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Permissions;
using static Handbook.Handbook;

namespace Handbook.Commands;

public static partial class Commands
{
    [Command("handbook edit", "handbook.edit.desc")]
    [CommandPermission("handbook.edit")]
    [CommandSyntax("en-US", "<document>", "<content>")]
    [CommandSyntax("ru-RU", "<документ>", "<содержание>")]
    public static void Edit(IAmethystUser _, CommandInvokeContext ctx, string doc, string content)
    {
        string docPath = Path.Combine(_handbookBasePath, doc);

        // Try to get the file without extension, expecting only .txt files
        if (!docPath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            docPath += ".txt";
        }

        // If doc does not exist, create after checking permission
        if (!File.Exists(docPath))
        {
            if (ctx.Permissions.HasPermission("handbook.create") != PermissionAccess.HasPermission)
            {
                ctx.Messages.ReplyError("commands.noPermission");
                return;
            }
        }

        // Set file text contents to content
        try
        {
            File.WriteAllText(docPath, content);
        }
        catch
        {
            ctx.Messages.ReplyError("commands.commandFailed");
            return;
        }

        ctx.Messages.ReplySuccess("handbook.edit.success", doc);
    }

    [Command(["handbook remove", "handbook rm"], "handbook.edit.desc")]
    [CommandPermission("handbook.remove")]
    [CommandSyntax("en-US", "<document>")]
    [CommandSyntax("ru-RU", "<документ>")]
    public static void Remove(IAmethystUser _, CommandInvokeContext ctx, string doc)
    {
        string docPath = Path.Combine(_handbookBasePath, doc);

        // Try to get the file without extension, expecting only .txt files
        if (!docPath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            docPath += ".txt";
        }

        // If doc does not exist, return error
        if (!File.Exists(docPath))
        {
            ctx.Messages.ReplyError("commands.objectNotFound");
            return;
        }

        // Remove file
        try
        {
            File.Delete(docPath);
        }
        catch
        {
            ctx.Messages.ReplyError("commands.commandFailed");
            return;
        }

        ctx.Messages.ReplySuccess("handbook.remove.success", doc);
    }
}