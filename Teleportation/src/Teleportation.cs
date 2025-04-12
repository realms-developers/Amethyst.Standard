using Amethyst.Extensions.Plugins;

namespace Teleportation;

public sealed class Teleportation : PluginInstance
{
    public override string Name => "Teleportation";

    public override Version Version => new(1, 0);

    protected override void Load() { }

    protected override void Unload() { }
}
