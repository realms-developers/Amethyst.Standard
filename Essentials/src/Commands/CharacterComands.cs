using Amethyst.Commands;
using Amethyst.Players;

namespace Amethyst.Essentials.Commands;

public static class CharacterCommands
{
    [ServerCommand(CommandType.Shared, "ssc reset", "essentials.desc.reset", "essentials.ssc.reset")]
    [CommandsSyntax("<name>")]
    public static void Reset(CommandInvokeContext ctx, string name)
    {
        bool removed = PlayerManager.Characters.Remove(name);
        if (!removed)
        {
            ctx.Sender.ReplyError(Localization.Get("essentials.text.characterNotFound", ctx.Sender.Language));
            return;
        }

        var model = PlayerManager.SSCProvider.GetModel(name);

        foreach (var plr in PlayerManager.Tracker)
        {
            if (plr.Name == name)
            {
                plr.Character?.LoadCharacter(model, true);
            }
        }

        ctx.Sender.ReplySuccess(Localization.Get("essentials.text.characterWasReseted", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "ssc savebak", "essentials.desc.backupSave", "essentials.ssc.savebak")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    [CommandsSyntax("<name>")]
    public static void SaveBackup(CommandInvokeContext ctx, string name)
    {
        var self = (ctx.Sender as NetPlayer)!;
        var character = self.Character!;

        EssentialsPlugin.CharactersBackup.Save(character.Model);
        ctx.Sender.ReplySuccess(Localization.Get("essentials.text.characterWasSaved", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "ssc loadbak", "essentials.desc.backupLoad", "essentials.ssc.loadbak")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    [CommandsSyntax("<name>")]
    public static void LoadBackup(CommandInvokeContext ctx, string name)
    {
        var self = (ctx.Sender as NetPlayer)!;

        var backup = EssentialsPlugin.CharactersBackup.Find(self.Name);
        if (backup == null)
        {
            ctx.Sender.ReplyError(Localization.Get("essentials.text.backupNotFound", ctx.Sender.Language));
            return;
        }

        ctx.Sender.ReplySuccess(Localization.Get("essentials.text.characterWasLoaded", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "ssc replace", "essentials.desc.replace", "essentials.ssc.replace")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    [CommandsSyntax("<name>")]
    public static void Replace(CommandInvokeContext ctx, string name)
    {
        var self = (ctx.Sender as NetPlayer)!;
        var model = self.Character!.Model;

        model.Name = name;
        model.Save();
        model.Name = self.Name;

        foreach (var plr in PlayerManager.Tracker)
        {
            if (plr.Name == name)
            {
                plr.Character?.LoadCharacter(PlayerManager.SSCProvider.GetModel(plr.Name), true);
            }
        }

        ctx.Sender.ReplySuccess(Localization.Get("essentials.text.characterWasReplaced", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "ssc clone", "essentials.desc.clone", "essentials.ssc.clone")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    [CommandsSyntax("<name>")]
    public static void Clone(CommandInvokeContext ctx, string name)
    {
        var self = (ctx.Sender as NetPlayer)!;

        var character = PlayerManager.SSCProvider.GetModel(name);
        if (character == null)
        {
            ctx.Sender.ReplyError(Localization.Get("essentials.text.backupNotFound", ctx.Sender.Language));
            return;
        }

        var backup = self.Character!.Model;
        backup.Name = "CHARACTER_BACKUP/" + backup.Name;
        EssentialsPlugin.CharactersBackup.Save(backup);

        self.Character!.LoadCharacter(character, true);

        ctx.Sender.ReplySuccess(Localization.Get("essentials.text.characterWasCloned", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "ssc restore", "essentials.desc.restore", "essentials.ssc.restore")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void Restore(CommandInvokeContext ctx)
    {
        var self = (ctx.Sender as NetPlayer)!;

        var backup = PlayerManager.SSCProvider.GetModel("CHARACTER_BACKUP/" + ctx.Sender.Name);
        if (backup == null)
        {
            ctx.Sender.ReplyError(Localization.Get("essentials.text.backupNotFound", ctx.Sender.Language));
            return;
        }

        backup.Name = ctx.Sender.Name;
        self.Character!.LoadCharacter(backup, true);

        ctx.Sender.ReplySuccess(Localization.Get("essentials.text.characterWasRestored", ctx.Sender.Language));
    }
}