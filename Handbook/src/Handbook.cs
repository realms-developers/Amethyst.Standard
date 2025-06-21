using Amethyst.Extensions.Base.Metadata;
using Amethyst.Extensions.Plugins;
using Amethyst.Kernel;
using Amethyst.Storages.Config;
using Handbook.Configuration;

namespace Handbook;

[ExtensionMetadata(nameof(Handbook), "realms-developers")]
public sealed class Handbook : PluginInstance
{
    internal static readonly string _handbookBasePath = Path.Combine(AmethystSession.Profile.SavePath, nameof(Handbook));
    internal static readonly Configuration<HandbookConfiguration> _hCfg = new(typeof(HandbookConfiguration).FullName!, new());

    protected override void Load() => _hCfg.Load();

    protected override void Unload() { }
}
