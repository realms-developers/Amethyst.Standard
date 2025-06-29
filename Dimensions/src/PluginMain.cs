using Amethyst.Extensions.Plugins;
using Amethyst.Extensions.Base.Metadata;
using Amethyst.Network.Handling;

namespace Dimensions;

[ExtensionMetadata("Dimensions", "realms-developers", "Provides connection with popstarfreas/Dimensions.")]
public sealed class PluginMain : PluginInstance
{
    private DimensionsHandler _dimensionsHandler = new();

    protected override void Load()
    {
        HandlerManager.RegisterHandler(_dimensionsHandler);
    }

    protected override void Unload()
    {
        HandlerManager.UnregisterHandler(_dimensionsHandler);
        _dimensionsHandler = null!;
    }
}
