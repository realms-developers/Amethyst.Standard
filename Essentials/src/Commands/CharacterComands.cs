using Amethyst;
using Amethyst.Commands;
using Amethyst.Commands.Attributes;
using Amethyst.Players;

namespace Essentials.Commands;

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

        Amethyst.Players.SSC.CharacterModel model = PlayerManager.SSCProvider.GetModel(name);

        foreach (NetPlayer plr in PlayerManager.Tracker)
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
    public static void SaveBackup(CommandInvokeContext ctx)
    {
        NetPlayer self = (ctx.Sender as NetPlayer)!;
        Amethyst.Players.SSC.Interfaces.ICharacterWrapper character = self.Character!;

        EssentialsPlugin.CharactersBackup.Save(character.Model);
        ctx.Sender.ReplySuccess(Localization.Get("essentials.text.characterWasSaved", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "ssc loadbak", "essentials.desc.backupLoad", "essentials.ssc.loadbak")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void LoadBackup(CommandInvokeContext ctx)
    {
        NetPlayer self = (ctx.Sender as NetPlayer)!;

        Amethyst.Players.SSC.CharacterModel? backup = EssentialsPlugin.CharactersBackup.Find(self.Name);
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
        NetPlayer self = (ctx.Sender as NetPlayer)!;
        Amethyst.Players.SSC.CharacterModel model = self.Character!.Model;

        model.Name = name;
        model.Save();
        model.Name = self.Name;

        foreach (NetPlayer plr in PlayerManager.Tracker)
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
        NetPlayer self = (ctx.Sender as NetPlayer)!;

        Amethyst.Players.SSC.CharacterModel character = PlayerManager.SSCProvider.GetModel(name);
        if (character == null)
        {
            ctx.Sender.ReplyError(Localization.Get("essentials.text.backupNotFound", ctx.Sender.Language));
            return;
        }

        Amethyst.Players.SSC.CharacterModel backup = self.Character!.Model;
        backup.Name = "CHARACTER_BACKUP/" + backup.Name;
        EssentialsPlugin.CharactersBackup.Save(backup);

        self.Character!.LoadCharacter(character, true);

        ctx.Sender.ReplySuccess(Localization.Get("essentials.text.characterWasCloned", ctx.Sender.Language));
    }

    [ServerCommand(CommandType.Shared, "ssc restore", "essentials.desc.restore", "essentials.ssc.restore")]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void Restore(CommandInvokeContext ctx)
    {
        NetPlayer self = (ctx.Sender as NetPlayer)!;

        Amethyst.Players.SSC.CharacterModel backup = PlayerManager.SSCProvider.GetModel("CHARACTER_BACKUP/" + ctx.Sender.Name);
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
