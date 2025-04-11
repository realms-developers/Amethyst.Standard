using Amethyst.Commands;
using Amethyst.Core;
using Amethyst.Extensions.Plugins;
using Amethyst.Players;
using Amethyst.Players.SSC;
using Amethyst.Storages.Mongo;

namespace Essentials;

public sealed class Essentials : PluginInstance
{
    public static MongoModels<CharacterModel> CharactersBackup { get; set; } = PlayerManager.Characters.Database.Get<CharacterModel>("CharactersBackupModelCollection");

    public override string Name => "Essentials";

    public override Version Version => new(1, 0);

    internal static string[] SSCCommands =
    [
        "ssc reset",
        "ssc savebak",
        "ssc loadbak",
        "ssc clone",
        "ssc replace",
    ];

    protected override void Load()
    {
        TryDisableCommands(PlayerManager.IsSSCEnabled == false, SSCCommands);
    }

    private static void TryDisableCommands(bool state, string[] commands)
    {
        if (!state)
        {
            return;
        }

        foreach (string cmd in commands)
        {
            CommandRunner? instanceCmd = CommandsManager.FindCommand(cmd);
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
