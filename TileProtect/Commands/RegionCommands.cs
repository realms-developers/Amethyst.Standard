using Amethyst.Commands;
using Amethyst.Players;
using Amethyst.Text;
using Amethyst.TileProtect.Extensions;
using Amethyst.TileProtect.Models;
using Microsoft.Xna.Framework;

namespace Amethyst.TileProtect.Commands;

public static class RegionCommands
{
    #region Basic management
    [ServerCommand(CommandType.Shared, "rg point", "$LOCALIZE region.setPoint", "amethyst.management.regions")]
    [CommandsSyntax("<1 | 2>", "[X:Y]")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void RegionPoint(CommandInvokeContext ctx, byte pointId, string? manual = null)
    {
        if (pointId > 2 || pointId < 1)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.invalidPoint");
            return;
        }

        Point? point = null;
        if (manual != null)
        {
            var splitted = manual.Split(':');
            if (splitted.Length == 2 && int.TryParse(splitted[0], out int x) && int.TryParse(splitted[1], out int y))
            {
                point = new Point(x, y);
            }
            else
            {
                ctx.Sender.ReplyError("$LOCALIZE region.invalidManualPoint");
                return;
            }
        }

        var ext = (ctx.Sender as NetPlayer)!.GetExtension<RegionPlayerExtension>()!;

        if (pointId == 1)
            ext.point1 = point;
        else
            ext.point2 = point;

        if (point != null)
        {
            ctx.Sender.ReplySuccess("$LOCALIZE region.manualPointSet");
            return;
        }

        ext.pointToDo = (byte)(pointId);
        ctx.Sender.ReplySuccess("$LOCALIZE region.hitBlock");
    }

    [ServerCommand(CommandType.Shared, "rg create", "$LOCALIZE region.createRegion", "amethyst.management.regions")]
    [CommandsSyntax("<name>")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void RegionCreate(CommandInvokeContext ctx, string name)
    {
        var ext = (ctx.Sender as NetPlayer)!.GetExtension<RegionPlayerExtension>()!;
        if (string.IsNullOrWhiteSpace(name))
        {
            ctx.Sender.ReplyError("$LOCALIZE region.invalidName");
            return;
        }

        if (ext.point1 == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.noPoint1");
            return;
        }

        if (ext.point2 == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.noPoint2");
            return;
        }

        RegionModel model = new RegionModel(name)
        {
            X = ext.point1.Value.X,
            Y = ext.point1.Value.Y,
            X2 = ext.point2.Value.X,
            Y2 = ext.point2.Value.Y,

            Owner = ctx.Sender.Name
        };

        model.Save();
        ctx.Sender.ReplySuccess("$LOCALIZE region.created");
    }

    [ServerCommand(CommandType.Shared, "rg list", "$LOCALIZE region.list", "amethyst.management.regions")]
    [CommandsSyntax("[page]")]
    public static void RegionList(CommandInvokeContext ctx, int page = 0)
    {
        PagesCollection pages = PagesCollection.CreateFromList(PagesCollection.PageifyItems(ProtectionModule.Regions.FindAll().Select(p => p.Name)));
        ctx.Sender.ReplyPage(pages, "$LOCALIZE region.listHeader", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "rg mlist", "$LOCALIZE region.listMembers", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[page]")]
    public static void RegionListMembers(CommandInvokeContext ctx, string name, int page = 0)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        PagesCollection pages = PagesCollection.CreateFromList(PagesCollection.PageifyItems(model.Members));
        ctx.Sender.ReplyPage(pages, "$LOCALIZE region.listMembersHeader", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "rg rm", "$LOCALIZE region.removeRegion", "amethyst.management.regions")]
    [CommandsSyntax("<name>")]
    public static void RegionRemove(CommandInvokeContext ctx, string name)
    {
        if (ProtectionModule.Regions.Remove(name))
        {
            ProtectionModule._cachedRegions = ProtectionModule.Regions.FindAll().ToList();
            ctx.Sender.ReplySuccess("$LOCALIZE region.removed");
        }
        else
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
        }
    }

    [ServerCommand(CommandType.Shared, "rg mvx", "$LOCALIZE region.moveX", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<offset>")]
    public static void RegionMoveX(CommandInvokeContext ctx, string name, int offset)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        model.X += offset;
        model.Save();
        
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg mvy", "$LOCALIZE region.moveY", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<offset>")]
    public static void RegionMoveY(CommandInvokeContext ctx, string name, int offset)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        model.Y += offset;
        model.Save();
        
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg mvx2", "$LOCALIZE region.moveX2", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<offset>")]
    public static void RegionMoveX2(CommandInvokeContext ctx, string name, int offset)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        model.X2 += offset;
        model.Save();
        
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg mvy2", "$LOCALIZE region.moveY2", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<offset>")]
    public static void RegionMoveY2(CommandInvokeContext ctx, string name, int offset)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        model.Y2 += offset;
        model.Save();
        
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg mvz", "$LOCALIZE region.moveZ", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<offset>")]
    public static void RegionMoveZ(CommandInvokeContext ctx, string name, int z)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        model.Z += z;
        model.Save();
        
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg setx", "$LOCALIZE region.setX", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<value>")]
    public static void RegionSetX(CommandInvokeContext ctx, string name, int value)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        model.X = value;
        model.Save();
        
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg sety", "$LOCALIZE region.setY", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<value>")]
    public static void RegionSetY(CommandInvokeContext ctx, string name, int value)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        model.Y = value;
        model.Save();
        
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg setx2", "$LOCALIZE region.setX2", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<value>")]
    public static void RegionSetX2(CommandInvokeContext ctx, string name, int value)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        model.X2 = value;
        model.Save();
        
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg sety2", "$LOCALIZE region.setY2", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<value>")]
    public static void RegionSetY2(CommandInvokeContext ctx, string name, int value)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        model.Y2 = value;
        model.Save();
        
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg setz", "$LOCALIZE region.setZ", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<value>")]
    public static void RegionSetZ(CommandInvokeContext ctx, string name, int z)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        model.Z = z;
        model.Save();
        
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg addmember", "$LOCALIZE region.addMember", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<username>")]
    public static void RegionAddMember(CommandInvokeContext ctx, string name, string member)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        model.Members.Add(name);
        model.Save();
        
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg rmmember", "$LOCALIZE region.removeMember", "amethyst.management.regions")]
    [CommandsSyntax("<name or *>", "<username>")]
    public static void RegionRemoveMember(CommandInvokeContext ctx, string name, string member)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        if (name == "*")
            model.Members.Clear();
        else
            model.Members.Remove(name);

        model.Save();
        
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg setowner", "$LOCALIZE region.setOwner", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<username>")]
    public static void RegionSetOwner(CommandInvokeContext ctx, string name, string owner)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        model.Owner = owner;
        model.Save();
        
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    #endregion

    #region Protection

    [ServerCommand(CommandType.Shared, "rg protect", "$LOCALIZE region.protect", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<tiles | (edit/open)(signs/chests) | tileEntities>", "<true | false>")]
    public static void RegionProtect(CommandInvokeContext ctx, string name, string flag, bool state)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        ProtectType type;
        if (Enum.TryParse<ProtectType>(flag, true, out type))
        {
            model.Protection[(int)type] = state;
            model.Save();
            
            ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
            return;
        }

        ctx.Sender.ReplyError("$LOCALIZE region.invalidFlag");
    }

    [ServerCommand(CommandType.Shared, "rg sprotect", "$LOCALIZE region.superProtect", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<tiles | (edit/open)(signs/chests) | tileEntities>", "<true | false>")]
    public static void RegionSuperProtect(CommandInvokeContext ctx, string name, string flag, bool state)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        ProtectType type;
        if (Enum.TryParse<ProtectType>(flag, true, out type))
        {
            model.SuperProtection[(int)type] = state;
            model.Save();
            
            ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
            return;
        }

        ctx.Sender.ReplyError("$LOCALIZE region.invalidFlag");
    }

    #endregion

    #region Entering

    [ServerCommand(CommandType.Shared, "rg emsglist", "$LOCALIZE region.enterMessagesList", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[page]")]
    public static void RegionEnterMessageList(CommandInvokeContext ctx, string name, int page = 0)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        string[] lines = new string[model.EnterMessages.Count];
        for (int i = 0; i < lines.Length; i++)
            lines[i] = $"{i}: {model.EnterMessages[i]}";
        
        var pages = PagesCollection.SplitByPages(lines, 10);
        ctx.Sender.ReplyPage(pages, "$LOCALIZE commands.enterMessagesListHeader", null, null, false, 0);
    }


    [ServerCommand(CommandType.Shared, "rg emsgins", "$LOCALIZE region.enterMessagesInsert", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<text>", "[index]")]
    public static void RegionEnterInsertMessage(CommandInvokeContext ctx, string name, string text, int index = -1)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        if (index >= 0)
            model.EnterMessages.Insert(index, text);
        else 
            model.EnterMessages.Add(text);

        model.Save();
            
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg emsgrm", "$LOCALIZE region.enterMessagesRemove", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[index (-1 for all)]")]
    public static void RegionEnterRemoveMessage(CommandInvokeContext ctx, string name, int index = -2)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        if (index == -1)
            model.EnterMessages.Clear();
        else if (index >= 0 && index < model.EnterMessages.Count)
            model.EnterMessages.RemoveAt(index);
        else
        {
            ctx.Sender.ReplyError("$LOCALIZE region.invalidIndex");
            return;
        }

        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg eclist", "$LOCALIZE region.enterCommandsList", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[page]")]
    public static void RegionEnterCommandList(CommandInvokeContext ctx, string name, int page = 0)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        string[] lines = new string[model.EnterCommands.Count];
        for (int i = 0; i < lines.Length; i++)
            lines[i] = $"{i}: {model.EnterCommands[i].Command} ({model.EnterCommands[i].CommandType}) [{model.EnterCommands[i].Permission ?? "no permission"}]";
        
        var pages = PagesCollection.SplitByPages(lines, 10);
        ctx.Sender.ReplyPage(pages, "$LOCALIZE commands.enterCommandsListHeader", null, null, false, 0);
    }

    [ServerCommand(CommandType.Shared, "rg ecins", "$LOCALIZE region.enterCommandsInsert", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<command>", "<console | self>", "[permission (or -)]", "[index]")]
    public static void RegionEnterInsertCommand(CommandInvokeContext ctx, string name, string command, string sender, string permission = "-", int index = -1)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        RegionCommandType type;
        if (Enum.TryParse<RegionCommandType>(sender, out type))
        {
            var rgCmd = new RegionCommand(type, command, permission == "-" ? null : permission);

            if (index >= 0)
                model.EnterCommands.Insert(index, rgCmd);
            else 
                model.EnterCommands.Add(rgCmd);

            model.Save();
            
            ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
            return;
        }

        ctx.Sender.ReplyError("$LOCALIZE region.invalidCommandType");
    }

    [ServerCommand(CommandType.Shared, "rg ecrm", "$LOCALIZE region.enterCommandsRemove", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[index (-1 for all)]")]
    public static void RegionEnterRemoveCommand(CommandInvokeContext ctx, string name, int index = -2)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        if (index == -1)
            model.EnterCommands.Clear();
        else if (index >= 0 && index < model.EnterCommands.Count)
            model.EnterCommands.RemoveAt(index);
        else
        {
            ctx.Sender.ReplyError("$LOCALIZE region.invalidIndex");
            return;
        }

        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    #endregion

    #region Leaving

    [ServerCommand(CommandType.Shared, "rg lmsglist", "$LOCALIZE region.leaveMessagesList", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[page]")]
    public static void RegionLeaveMessageList(CommandInvokeContext ctx, string name, int page = 0)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        string[] lines = new string[model.LeaveMessages.Count];
        for (int i = 0; i < lines.Length; i++)
            lines[i] = $"{i}: {model.LeaveMessages[i]}";
        
        var pages = PagesCollection.SplitByPages(lines, 10);
        ctx.Sender.ReplyPage(pages, "$LOCALIZE commands.leaveMessagesListHeader", null, null, false, 0);
    }


    [ServerCommand(CommandType.Shared, "rg lmsgins", "$LOCALIZE region.leaveMessagesInsert", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<text>", "[index]")]
    public static void RegionLeaveInsertMessage(CommandInvokeContext ctx, string name, string text, int index = -1)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        if (index >= 0)
            model.LeaveMessages.Insert(index, text);
        else 
            model.LeaveMessages.Add(text);

        model.Save();
            
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg lmsgrm", "$LOCALIZE region.leaveMessagesRemove", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[index (-1 for all)]")]
    public static void RegionLeaveRemoveMessage(CommandInvokeContext ctx, string name, int index = -2)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        if (index == -1)
            model.LeaveMessages.Clear();
        else if (index >= 0 && index < model.LeaveMessages.Count)
            model.LeaveMessages.RemoveAt(index);
        else
        {
            ctx.Sender.ReplyError("$LOCALIZE region.invalidIndex");
            return;
        }

        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg lclist", "$LOCALIZE region.leaveCommandsList", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<command>", "<console | self>", "[permission (or -)]", "[index]")]
    public static void RegionLeaveCommandList(CommandInvokeContext ctx, string name, string command, string sender, string permission = "-", int index = -1)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        string[] lines = new string[model.LeaveCommands.Count];
        for (int i = 0; i < lines.Length; i++)
            lines[i] = $"{i}: {model.LeaveCommands[i].Command} ({model.LeaveCommands[i].CommandType}) [{model.LeaveCommands[i].Permission ?? "no permission"}]";
        
        ctx.Sender.ReplyError("$LOCALIZE region.invalidCommandType");
    }

    [ServerCommand(CommandType.Shared, "rg lcins", "$LOCALIZE region.leaveCommandsInsert", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<command>", "<console | self>", "[permission (or -)]", "[index]")]
    public static void RegionLeaveInsertCommand(CommandInvokeContext ctx, string name, string command, string sender, string permission = "-", int index = -1)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        RegionCommandType type;
        if (Enum.TryParse<RegionCommandType>(sender, out type))
        {
            var rgCmd = new RegionCommand(type, command, permission == "-" ? null : permission);

            if (index >= 0)
                model.LeaveCommands.Insert(index, rgCmd);
            else 
                model.LeaveCommands.Add(rgCmd);

            model.Save();
            
            ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
            return;
        }

        ctx.Sender.ReplyError("$LOCALIZE region.invalidCommandType");
    }

    [ServerCommand(CommandType.Shared, "rg lcrm", "$LOCALIZE region.leaveCommandsRemove", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "[index (-1 for all)]")]
    public static void RegionLeaveRemoveCommand(CommandInvokeContext ctx, string name, int index = -2)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        if (index == -1)
            model.LeaveCommands.Clear();
        else if (index >= 0 && index < model.EnterCommands.Count)
            model.LeaveCommands.RemoveAt(index);
        else
        {
            ctx.Sender.ReplyError("$LOCALIZE region.invalidIndex");
            return;
        }

        model.Save();

        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    #endregion

    #region Flags

    [ServerCommand(CommandType.Shared, "rg autogodmode", "$LOCALIZE region.autogodmode", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<true | false>")]
    public static void RegionAutoGodMode(CommandInvokeContext ctx, string name, bool state)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        model.AutoGodMode = state;
        model.Save();
            
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg noitems", "$LOCALIZE region.noitems", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<true | false>")]
    public static void RegionNoItems(CommandInvokeContext ctx, string name, bool state)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        model.AutoGodMode = state;
        model.Save();
            
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    [ServerCommand(CommandType.Shared, "rg noenemies", "$LOCALIZE region.noenemies", "amethyst.management.regions")]
    [CommandsSyntax("<name>", "<true | false>")]
    public static void RegionNoEnemies(CommandInvokeContext ctx, string name, bool state)
    {
        var model = ProtectionModule.Regions.Find(name);
        if (model == null)
        {
            ctx.Sender.ReplyError("$LOCALIZE region.notFound");
            return;
        }

        model.NoEnemies = state;
        model.Save();
            
        ctx.Sender.ReplySuccess("$LOCALIZE region.modified");
    }

    #endregion
}