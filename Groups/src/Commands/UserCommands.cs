using Amethyst.Commands;
using Amethyst.Commands.Attributes;
using Amethyst.Text;
using Groups.Models;

namespace Groups.Commands;

public static class UserCommands
{
    [ServerCommand(CommandType.Shared, "um permedlist", "$LOCALIZE users.permedlist.command", "amethyst.management.groups")]
    [CommandsSyntax("[page]")]
    public static void UserPermissionedList(CommandInvokeContext ctx, int page = 0)
    {
        List<string> list = PagesCollection.PageifyItems(Groups.Users.FindAll(p => p.PersonalPermissions.Any()).Select(p => $"{p.Name} ({p.PersonalPermissions.Count})"), 80);
        var pages = PagesCollection.SplitByPages(list, 10);

        ctx.Sender.ReplyPage(pages, "$LOCALIZE users.permed", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "um ppermlist", "$LOCALIZE users.ppermlist.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "[page]")]
    public static void UserPermissionList(CommandInvokeContext ctx, string name, int page = 0)
    {
        Models.GroupUserModel? model = Groups.Users.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE users.userNotFound", name);
            return;
        }

        List<string> list = PagesCollection.PageifyItems(model.PersonalPermissions, 80);
        var pages = PagesCollection.SplitByPages(list, 10);

        ctx.Sender.ReplyPage(pages, "$LOCALIZE users.personalPerms", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "um addperm", "$LOCALIZE users.addperm.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "<permission>")]
    public static void UserAddPermission(CommandInvokeContext ctx, string name, string perm)
    {
        Models.GroupUserModel? model = Groups.Users.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE users.userNotFound", name);
            return;
        }

        model.PersonalPermissions.Add(perm);

        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE users.permissionsModified");
    }

    [ServerCommand(CommandType.Shared, "um rmperm", "$LOCALIZE users.rmperm.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "<permission or *>")]
    public static void UserRemovePermission(CommandInvokeContext ctx, string name, string perm)
    {
        Models.GroupUserModel? model = Groups.Users.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE users.userNotFound", name);
            return;
        }

        if (perm == "*")
        {
            model.PersonalPermissions.Clear();
        }
        else
        {
            model.PersonalPermissions.RemoveAll(p => p == perm);
        }

        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE users.permissionsModified");
    }

    [ServerCommand(CommandType.Shared, "um set", "$LOCALIZE users.set.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "<group>", "[time d/h/m/s]")]
    public static void UserSet(CommandInvokeContext ctx, string name, string groupName, string? time = null)
    {
        Models.GroupUserModel? model = Groups.Users.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE users.userNotFound", name);
            return;
        }

        Models.GroupModel? groupModel = Groups.GroupModels.Find(groupName);
        if (groupModel == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE groups.groupNotFound", groupName);
            return;
        }

        int? expiration = time == null ? null : TextUtility.ParseToSeconds(time);
        if (expiration != null)
        {
            model.TempGroup = new TempGroup(groupName, DateTime.UtcNow.AddSeconds(expiration.Value));
        }
        else
        {
            model.Group = groupName;
        }

        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE users.groupModified");
    }

    [ServerCommand(CommandType.Shared, "um unset", "$LOCALIZE users.unset.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "<direct, temp, all>")]
    public static void UserUnset(CommandInvokeContext ctx, string name, string arg)
    {
        Models.GroupUserModel? model = Groups.Users.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE users.userNotFound", name);
            return;
        }

        switch (arg.ToLowerInvariant())
        {
            case "direct":
                model.Group = null;
                break;
            case "temp":
                model.TempGroup = null;
                break;
            case "all":
                model.Group = null;
                model.TempGroup = null;
                break;
            default:
                ctx.Sender.ReplyError("$LOCALIZE users.invalidArgument");
                return;
        }

        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE users.groupModified");
    }
}
