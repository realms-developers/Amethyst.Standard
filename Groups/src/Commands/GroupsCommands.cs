using Amethyst.Commands;
using Amethyst.Groups.Models;
using Amethyst.Text;

namespace Amethyst.Groups.Commands;

public static class GroupsCommands
{
    [ServerCommand(CommandType.Shared, "gm init", "$LOCALIZE groups.init.command", "amethyst.management.groups")]
    public static void GroupInit(CommandInvokeContext ctx)
    {
        new GroupModel("operator") 
        {
            Permissions = new List<string>() { "operator" },
            Color = new Network.NetColor(255, 0, 0),
            Prefix = "[Operator]",
            BlockTempGroup = true
        }.Save();

        new GroupModel("def") 
        {
            Permissions = new List<string>() { "def" },
            Color = new Network.NetColor(155, 155, 155),
            Prefix = "[Default]",
            IsDefault = true
        }.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE groups.initGroupResult");
    }

    [ServerCommand(CommandType.Shared, "gm popexclude", "$LOCALIZE groups.reset.command", "amethyst.management.groups")]
    [CommandsSyntax("<group1,group2 ...>")]
    public static void GroupReset(CommandInvokeContext ctx, string names)
    {
        string[] splitted = names?.Split() ?? Array.Empty<string>();
        GroupsModule.Groups.Remove(p => splitted.Contains(p.Name) == false);
        GroupsModule.Reload();

        ctx.Sender.ReplySuccess("$LOCALIZE groups.resetGroupResult");
    }

    [ServerCommand(CommandType.Shared, "gm push", "$LOCALIZE groups.push.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "[permission1,permission2 ...]")]
    public static void GroupPush(CommandInvokeContext ctx, string name, string? permissions = null)
    {
        GroupModel model = new GroupModel(name);
        if (permissions != null)
        {
            string[] splitted = permissions.Split(',');

            foreach (var str in splitted)
                if (str != null)
                    model.Permissions.Add(str);
        }

        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE groups.pushGroupResult", model.Name);
    }

    [ServerCommand(CommandType.Shared, "gm pop", "$LOCALIZE groups.pop.command", "amethyst.management.groups")]
    [CommandsSyntax("<group1,group2 ...>")]
    public static void GroupPop(CommandInvokeContext ctx, string names)
    {
        string[] splitted = names?.Split() ?? Array.Empty<string>();
        GroupsModule.Groups.Remove(p => splitted.Contains(p.Name));
        GroupsModule.Reload();

        ctx.Sender.ReplySuccess("$LOCALIZE groups.popGroupResult");
    }

    [ServerCommand(CommandType.Shared, "gm addperm", "$LOCALIZE groups.addperm.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "<permission1,permission2 ...>")]
    public static void GroupAddPerm(CommandInvokeContext ctx, string name, string permissions)
    {
        var model = GroupsModule.Groups.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE groups.groupNotFound", name);
            return;
        }
        
        foreach (var str in permissions.Split(','))
            if (str != null)
                model.Permissions.Add(str);

        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE groups.addpermGroupResult", name);
    }

    [ServerCommand(CommandType.Shared, "gm rmperm", "$LOCALIZE groups.removeperm.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "<permission1,permission2 ...>")]
    public static void GroupRemovePerm(CommandInvokeContext ctx, string name, string permissions)
    {
        var model = GroupsModule.Groups.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE groups.groupNotFound", name);
            return;
        }
        
        foreach (var str in permissions.Split(','))
            if (str != null)
                model.Permissions.Remove(str);

        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE groups.rmpermGroupResult", name);
    }

    [ServerCommand(CommandType.Shared, "gm setdef", "$LOCALIZE groups.setdef.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "<true/false>")]
    public static void GroupSetDefault(CommandInvokeContext ctx, string name, bool value)
    {
        var model = GroupsModule.Groups.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE groups.groupNotFound", name);
            return;
        }
        
        model.IsDefault = value;
        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE groups.setdefGroupResult", name, value);
    }

    [ServerCommand(CommandType.Shared, "gm btemp", "$LOCALIZE groups.btemp.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "<true/false>")]
    public static void GroupBlockTemp(CommandInvokeContext ctx, string name, bool value)
    {
        var model = GroupsModule.Groups.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE groups.groupNotFound", name);
            return;
        }
        
        model.BlockTempGroup = value;
        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE groups.btempGroupResult", name, value);
    }

    [ServerCommand(CommandType.Shared, "gm parent", "$LOCALIZE groups.parent.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "[parent]")]
    public static void GroupParent(CommandInvokeContext ctx, string name, string? parent = null)
    {
        var model = GroupsModule.Groups.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE groups.groupNotFound", name);
            return;
        }
        
        model.Parent = parent;
        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE groups.parentGroupResult", name, parent ?? "-");
    }

    [ServerCommand(CommandType.Shared, "gm rgb", "$LOCALIZE groups.rgb.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "<R>", "<G>", "<B>")]
    public static void GroupParent(CommandInvokeContext ctx, string name, byte r, byte g, byte b)
    {
        var model = GroupsModule.Groups.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE groups.groupNotFound", name);
            return;
        }
        
        model.Color = new Network.NetColor(r, g, b);
        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE groups.rgbGroupResult", name, r, g, b);
    }

    [ServerCommand(CommandType.Shared, "gm pfx", "$LOCALIZE groups.pfx.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "[prefix]")]
    public static void GroupPrefix(CommandInvokeContext ctx, string name, string? prefix = null)
    {
        var model = GroupsModule.Groups.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE groups.groupNotFound", name);
            return;
        }
        
        model.Prefix = prefix;
        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE groups.pfxGroupResult", name, prefix ?? "-");
    }

    [ServerCommand(CommandType.Shared, "gm sfx", "$LOCALIZE groups.sfx.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "[prefix]")]
    public static void GroupSuffix(CommandInvokeContext ctx, string name, string? suffix = null)
    {
        var model = GroupsModule.Groups.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE groups.groupNotFound", name);
            return;
        }
        
        model.Suffix = suffix;
        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE groups.sfxGroupResult", name, suffix ?? "-");
    }

    [ServerCommand(CommandType.Shared, "gm usrlist", "$LOCALIZE groups.usrlist.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "[page]")]
    public static void GroupUsersList(CommandInvokeContext ctx, string name, int page = 0)
    {
        var model = GroupsModule.Groups.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE groups.groupNotFound", name);
            return;
        }
        
        var list = PagesCollection.PageifyItems(GroupsModule.Users.FindAll(p => p.Group == name).Select(p => p.Name), 80);
        var pages = PagesCollection.SplitByPages(list, 10);

        ctx.Sender.ReplyPage(pages, "$LOCALIZE groups.users", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "gm permlist", "$LOCALIZE groups.permlist.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "[page]")]
    public static void GroupPermList(CommandInvokeContext ctx, string name, int page = 0)
    {
        var model = GroupsModule.Groups.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE groups.groupNotFound", name);
            return;
        }
        
        var list = PagesCollection.PageifyItems(model.Permissions, 80);
        var pages = PagesCollection.SplitByPages(list, 10);

        ctx.Sender.ReplyPage(pages, "$LOCALIZE groups.permissions", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "gm grlist", "$LOCALIZE groups.cmdlist.command", "amethyst.management.groups")]
    [CommandsSyntax("<name>", "[page]")]
    public static void GroupCommandList(CommandInvokeContext ctx, string name, int page = 0)
    {
        var list = PagesCollection.PageifyItems(GroupsModule.Groups.FindAll().Select(p => p.Name), 80);
        var pages = PagesCollection.SplitByPages(list, 10);

        ctx.Sender.ReplyPage(pages, "$LOCALIZE groups.groups", null, null, false, page);
    }
}