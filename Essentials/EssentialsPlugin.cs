using Amethyst.Commands;
using Amethyst.Core;
using Amethyst.Extensions.Plugins;
using Amethyst.Players;
using Amethyst.Players.Extensions;
using Amethyst.Players.SSC;
using Amethyst.Storages.Mongo;

namespace Amethyst.Essentials;

public sealed class EssentialsPlugin : PluginInstance
{
    public static MongoModels<CharacterModel> CharactersBackup = PlayerManager.Characters.Database.Get<CharacterModel>("CharactersBackupModelCollection");

    public override string Name => "Essentials";

    public override Version Version => new Version(1, 0);

    internal static string[] SSCCommands = new string[]
    {
        "ssc reset",
        "ssc savebak",
        "ssc loadbak",
        "ssc clone",
        "ssc replace",
    };

    protected override void Load()
    {
        TryDisableCommands(PlayerManager.IsSSCEnabled == false, SSCCommands);
    }

    private void TryDisableCommands(bool state, string[] commands)
    {
        if (!state) return;

        foreach (var cmd in commands)
        {
            var instanceCmd = CommandsManager.FindCommand(cmd);
            if (instanceCmd == null)
            {
                AmethystLog.Main.Error("Amethyst.Essentials", $"Cannot find command {cmd}");
                continue;
            }
            instanceCmd.IsDisabled = true;
        }
    }

    protected override void Unload()
    {
    }
}
