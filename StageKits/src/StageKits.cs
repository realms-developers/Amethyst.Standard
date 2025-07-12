using Amethyst.Extensions.Plugins;
using Amethyst.Extensions.Base.Metadata;
using Amethyst.Systems.Characters;
using Amethyst.Systems.Characters.Base.Factories;
using Terraria;

namespace StageKits;

[ExtensionMetadata("StageKits", "realms-developers", "provides start kits for new characters by game stage")]
public sealed class StageKits : PluginInstance
{
    private IDefaultModelFactory _oldFactory = null!;
    protected override void Load()
    {
        StageKitsConfiguration.Configuration.Load();

        _oldFactory = CharactersOrganizer.ServersideFactory.ModelFactory;
        CharactersOrganizer.ServersideFactory.ModelFactory = new StageModelFactory();
    }

    protected override void Unload()
    {
        CharactersOrganizer.ServersideFactory.ModelFactory = _oldFactory;
        _oldFactory = null!;
    }
}
