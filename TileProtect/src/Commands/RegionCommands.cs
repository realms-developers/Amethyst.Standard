using Amethyst;
using Amethyst.Commands;
using Amethyst.Commands.Attributes;
using Amethyst.Players;
using Amethyst.Text;
using Microsoft.Xna.Framework;
using TileProtect.Extensions;
using TileProtect.Models;

namespace TileProtect.Commands;

public static class RegionCommands
{
    #region Basic management
    [ServerCommand(CommandType.Shared, "rg point", "region.setPoint", "amethyst.management.regions")]
    [CommandsSyntax("<1 | 2>", "[X:Y]")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void RegionPoint(CommandInvokeContext ctx, byte pointId, string? manual = null)
    {
        if (pointId > 2 || pointId < 1)
        {
            ctx.Sender.ReplyError(Localization.Get("region.invalidPoint", ctx.Sender.Language));
            return;
        }

        Point? point = null;
        if (manual != null)
        {
            string[] splitted = manual.Split(':');
            if (splitted.Length == 2 && int.TryParse(splitted[0], out int x) && int.TryParse(splitted[1], out int y))
            {
                point = new Point(x, y);
            }
            else
            {
                ctx.Sender.ReplyError(Localization.Get("region.invalidManualPoint", ctx.Sender.Language));
                return;
            }
        }

        RegionPlayerExtension ext = (ctx.Sender as NetPlayer)!.GetExtension<RegionPlayerExtension>()!;

        if (pointId == 1)
        {
            ext.point1 = point;
        }
        else
        {
            ext.point2 = point;
        }

        if (point != null)
        {
            ctx.Sender.ReplySuccess(
                string.Format(Localization.Get("region.manualPointSet", ctx.Sender.Language), pointId, point.Value.X, point.Value.Y));

            return;
        }

        ext.pointToDo = pointId;
        ctx.Sender.ReplySuccess(
            string.Format(Localization.Get("region.hitBlock", ctx.Sender.Language), pointId));
    }

    [ServerCommand(CommandType.Shared, "rg create", "region.createRegion", "amethyst.management.regions")]
    [CommandsSyntax("<name>")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void RegionCreate(CommandInvokeContext ctx, string name)
    {
        RegionPlayerExtension ext = (ctx.Sender as NetPlayer)!.GetExtension<RegionPlayerExtension>()!;
        if (string.IsNullOrWhiteSpace(name))
        {
            ctx.Sender.ReplyError(Localization.Get("region.invalidName", ctx.Sender.Language));
            return;
        }

        if (ext.point1 == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.noPoint1", ctx.Sender.Language));
            return;
        }

        if (ext.point2 == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.noPoint2", ctx.Sender.Language));
            return;
        }

        RegionModel model = new(name)
        {
            X = ext.point1.Value.X,
            Y = ext.point1.Value.Y,
            X2 = ext.point2.Value.X,
            Y2 = ext.point2.Value.Y,

            Owner = ctx.Sender.Name
        };

        model.Save();
        ctx.Sender.ReplySuccess(
            string.Format(Localization.Get("region.created", ctx.Sender.Language), name));
    }

    [ServerCommand(CommandType.Shared, "rg list", "region.list", "amethyst.management.regions")]
    [CommandsSyntax("[page]")]
    public static void RegionList(CommandInvokeContext ctx, int page = 0)
    {
        PagesCollection pages = PagesCollection.CreateFromList(PagesCollection.PageifyItems(ProtectionModule.Regions.FindAll().Select(p => p.Name)));
        ctx.Sender.ReplyPage(pages,
            string.Format(Localization.Get("region.listHeader", ctx.Sender.Language), page, pages.Count()),
            null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "rg mlist", "region.listMembers", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[page]")]
    public static void RegionListMembers(CommandInvokeContext ctx, string name, int page = 0)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        PagesCollection pages = PagesCollection.CreateFromList(PagesCollection.PageifyItems(model.Members));
        ctx.Sender.ReplyPage(pages,
            string.Format(Localization.Get("region.listMembersHeader", ctx.Sender.Language), name, page, pages.Count()),
            null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "rg rm", "region.removeRegion", "amethyst.management.regions")]
    [CommandsSyntax("<name>")]
    public static void RegionRemove(CommandInvokeContext ctx, string name)
    {
        if (ProtectionModule.Regions.Remove(name))
        {
            ProtectionModule._cachedRegions = [.. ProtectionModule.Regions.FindAll()];
            ctx.Sender.ReplySuccess(Localization.Get("region.removed", ctx.Sender.Language));
        }
        else
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
        }
    }

    [ServerCommand(CommandType.Shared, "rg mvx", "region.moveX", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<offset>")]
    public static void RegionMoveX(CommandInvokeContext ctx, string name, int offset)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        model.X += offset;
        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg mvy", "region.moveY", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<offset>")]
    public static void RegionMoveY(CommandInvokeContext ctx, string name, int offset)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        model.Y += offset;
        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg mvx2", "region.moveX2", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<offset>")]
    public static void RegionMoveX2(CommandInvokeContext ctx, string name, int offset)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        model.X2 += offset;
        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg mvy2", "region.moveY2", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<offset>")]
    public static void RegionMoveY2(CommandInvokeContext ctx, string name, int offset)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        model.Y2 += offset;
        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg mvz", "region.moveZ", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<offset>")]
    public static void RegionMoveZ(CommandInvokeContext ctx, string name, int z)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        model.Z += z;
        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg setx", "region.setX", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<value>")]
    public static void RegionSetX(CommandInvokeContext ctx, string name, int value)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        model.X = value;
        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg sety", "region.setY", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<value>")]
    public static void RegionSetY(CommandInvokeContext ctx, string name, int value)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        model.Y = value;
        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg setx2", "region.setX2", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<value>")]
    public static void RegionSetX2(CommandInvokeContext ctx, string name, int value)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        model.X2 = value;
        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg sety2", "region.setY2", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<value>")]
    public static void RegionSetY2(CommandInvokeContext ctx, string name, int value)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        model.Y2 = value;
        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg setz", "region.setZ", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<value>")]
    public static void RegionSetZ(CommandInvokeContext ctx, string name, int z)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        model.Z = z;
        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg addmember", "region.addMember", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<username>")]
    public static void RegionAddMember(CommandInvokeContext ctx, string name, string member)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        model.Members.Add(member); // Used to be 'name' instead of 'member', fixed?
        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg rmmember", "region.removeMember", "amethyst.management.regions")]
    [CommandsSyntax("<name or *>", "<username>")]
    public static void RegionRemoveMember(CommandInvokeContext ctx, string name, string member)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        // Used to be 'name' instead of 'member', fixed?
        if (member == "*")
        {
            model.Members.Clear();
        }
        else
        {
            model.Members.Remove(member);
        }

        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg setowner", "region.setOwner", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<username>")]
    public static void RegionSetOwner(CommandInvokeContext ctx, string name, string owner)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        model.Owner = owner;
        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    #endregion

    #region Protection

    [ServerCommand(CommandType.Shared, "rg protect", "region.protect", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<tiles | (edit/open)(signs/chests) | tileEntities>", "<true | false>")]
    public static void RegionProtect(CommandInvokeContext ctx, string name, string flag, bool state)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        if (Enum.TryParse(flag, true, out ProtectType type))
        {
            model.Protection[(int)type] = state;
            model.Save();

            ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
            return;
        }

        ctx.Sender.ReplyError(Localization.Get("region.invalidFlag", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg sprotect", "region.superProtect", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<tiles | (edit/open)(signs/chests) | tileEntities>", "<true | false>")]
    public static void RegionSuperProtect(CommandInvokeContext ctx, string name, string flag, bool state)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        if (Enum.TryParse(flag, true, out ProtectType type))
        {
            model.SuperProtection[(int)type] = state;
            model.Save();

            ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
            return;
        }

        ctx.Sender.ReplyError(Localization.Get("region.invalidFlag", ctx.Sender.Language));
    }

    #endregion

    #region Entering

    [ServerCommand(CommandType.Shared, "rg emsglist", "region.enterMessagesList", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[page]")]
    public static void RegionEnterMessageList(CommandInvokeContext ctx, string name, int page = 0)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        string[] lines = new string[model.EnterMessages.Count];
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = $"{i}: {model.EnterMessages[i]}";
        }

        var pages = PagesCollection.SplitByPages(lines, 10);
        ctx.Sender.ReplyPage(pages,
            string.Format(Localization.Get("commands.enterMessagesListHeader", ctx.Sender.Language), name, page, pages.Count()),
            null, null, false, 0);
    }


    [ServerCommand(CommandType.Shared, "rg emsgins", "region.enterMessagesInsert", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<text>", "[index]")]
    public static void RegionEnterInsertMessage(CommandInvokeContext ctx, string name, string text, int index = -1)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        if (index >= 0)
        {
            model.EnterMessages.Insert(index, text);
        }
        else
        {
            model.EnterMessages.Add(text);
        }

        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg emsgrm", "region.enterMessagesRemove", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[index (-1 for all)]")]
    public static void RegionEnterRemoveMessage(CommandInvokeContext ctx, string name, int index = -2)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        if (index == -1)
        {
            model.EnterMessages.Clear();
        }
        else if (index >= 0 && index < model.EnterMessages.Count)
        {
            model.EnterMessages.RemoveAt(index);
        }
        else
        {
            ctx.Sender.ReplyError(Localization.Get("region.invalidIndex", ctx.Sender.Language));
            return;
        }

        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg eclist", "region.enterCommandsList", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[page]")]
    public static void RegionEnterCommandList(CommandInvokeContext ctx, string name, int page = 0)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        string[] lines = new string[model.EnterCommands.Count];
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = $"{i}: {model.EnterCommands[i].Command} ({model.EnterCommands[i].CommandType}) [{model.EnterCommands[i].Permission ?? "no permission"}]";
        }

        var pages = PagesCollection.SplitByPages(lines, 10);
        ctx.Sender.ReplyPage(pages,
            string.Format(Localization.Get("commands.enterCommandsListHeader", ctx.Sender.Language), name, page, pages.Count()),
            null, null, false, 0);
    }

    [ServerCommand(CommandType.Shared, "rg ecins", "region.enterCommandsInsert", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<command>", "<console | self>", "[permission (or -)]", "[index]")]
    public static void RegionEnterInsertCommand(CommandInvokeContext ctx, string name, string command, string sender, string permission = "-", int index = -1)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        if (Enum.TryParse(sender, out RegionCommandType type))
        {
            var rgCmd = new RegionCommand(type, command, permission == "-" ? null : permission);

            if (index >= 0)
            {
                model.EnterCommands.Insert(index, rgCmd);
            }
            else
            {
                model.EnterCommands.Add(rgCmd);
            }

            model.Save();

            ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
            return;
        }

        ctx.Sender.ReplyError(Localization.Get("region.invalidCommandType", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg ecrm", "region.enterCommandsRemove", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[index (-1 for all)]")]
    public static void RegionEnterRemoveCommand(CommandInvokeContext ctx, string name, int index = -2)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        if (index == -1)
        {
            model.EnterCommands.Clear();
        }
        else if (index >= 0 && index < model.EnterCommands.Count)
        {
            model.EnterCommands.RemoveAt(index);
        }
        else
        {
            ctx.Sender.ReplyError(Localization.Get("region.invalidIndex", ctx.Sender.Language));
            return;
        }

        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    #endregion

    #region Leaving

    [ServerCommand(CommandType.Shared, "rg lmsglist", "region.leaveMessagesList", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[page]")]
    public static void RegionLeaveMessageList(CommandInvokeContext ctx, string name, int page = 0)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        string[] lines = new string[model.LeaveMessages.Count];
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = $"{i}: {model.LeaveMessages[i]}";
        }

        var pages = PagesCollection.SplitByPages(lines, 10);
        ctx.Sender.ReplyPage(pages,
            string.Format(Localization.Get("commands.leaveMessagesListHeader", ctx.Sender.Language), name, page, pages.Count()),
            null, null, false, 0);
    }


    [ServerCommand(CommandType.Shared, "rg lmsgins", "region.leaveMessagesInsert", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<text>", "[index]")]
    public static void RegionLeaveInsertMessage(CommandInvokeContext ctx, string name, string text, int index = -1)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        if (index >= 0)
        {
            model.LeaveMessages.Insert(index, text);
        }
        else
        {
            model.LeaveMessages.Add(text);
        }

        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg lmsgrm", "region.leaveMessagesRemove", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[index (-1 for all)]")]
    public static void RegionLeaveRemoveMessage(CommandInvokeContext ctx, string name, int index = -2)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        if (index == -1)
        {
            model.LeaveMessages.Clear();
        }
        else if (index >= 0 && index < model.LeaveMessages.Count)
        {
            model.LeaveMessages.RemoveAt(index);
        }
        else
        {
            ctx.Sender.ReplyError(Localization.Get("region.invalidIndex", ctx.Sender.Language));
            return;
        }

        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg lclist", "region.leaveCommandsList", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<command>", "<console | self>", "[permission (or -)]", "[index]")]
    public static void RegionLeaveCommandList(CommandInvokeContext ctx, string name, string _, string __, string ___ = "-", int ____ = -1)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        string[] lines = new string[model.LeaveCommands.Count];
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = $"{i}: {model.LeaveCommands[i].Command} ({model.LeaveCommands[i].CommandType}) [{model.LeaveCommands[i].Permission ?? "no permission"}]";
        }

        ctx.Sender.ReplyError(Localization.Get("region.invalidCommandType", ctx.Sender.Language));

        // TODO?
    }

    [ServerCommand(CommandType.Shared, "rg lcins", "region.leaveCommandsInsert", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<command>", "<console | self>", "[permission (or -)]", "[index]")]
    public static void RegionLeaveInsertCommand(CommandInvokeContext ctx, string name, string command, string sender, string permission = "-", int index = -1)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        if (Enum.TryParse(sender, out RegionCommandType type))
        {
            var rgCmd = new RegionCommand(type, command, permission == "-" ? null : permission);

            if (index >= 0)
            {
                model.LeaveCommands.Insert(index, rgCmd);
            }
            else
            {
                model.LeaveCommands.Add(rgCmd);
            }

            model.Save();

            ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
            return;
        }

        ctx.Sender.ReplyError(Localization.Get("region.invalidCommandType", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg lcrm", "region.leaveCommandsRemove", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[index (-1 for all)]")]
    public static void RegionLeaveRemoveCommand(CommandInvokeContext ctx, string name, int index = -2)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        if (index == -1)
        {
            model.LeaveCommands.Clear();
        }
        else if (index >= 0 && index < model.EnterCommands.Count)
        {
            model.LeaveCommands.RemoveAt(index);
        }
        else
        {
            ctx.Sender.ReplyError(Localization.Get("region.invalidIndex", ctx.Sender.Language));
            return;
        }

        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    #endregion

    #region Flags

    [ServerCommand(CommandType.Shared, "rg autogodmode", "region.autogodmode", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<true | false>")]
    public static void RegionAutoGodMode(CommandInvokeContext ctx, string name, bool state)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        model.AutoGodMode = state;
        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg noitems", "region.noitems", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<true | false>")]
    public static void RegionNoItems(CommandInvokeContext ctx, string name, bool state)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        model.AutoGodMode = state;
        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "rg noenemies", "region.noenemies", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<true | false>")]
    public static void RegionNoEnemies(CommandInvokeContext ctx, string name, bool state)
    {
        RegionModel? model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError(Localization.Get("region.notFound", ctx.Sender.Language));
            return;
        }

        model.NoEnemies = state;
        model.Save();

        ctx.Sender.ReplySuccess(Localization.Get("region.modified", ctx.Sender.Language));
    }

    #endregion
}
