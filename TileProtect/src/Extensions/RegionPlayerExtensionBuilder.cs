using Amethyst.Players;
using Amethyst.Players.Extensions;

namespace Amethyst.TileProtect.Extensions;

public sealed class RegionPlayerExtensionBuilder : IPlayerExtensionBuilder<RegionPlayerExtension>
{
    public RegionPlayerExtension Build(NetPlayer player)
    {
        return new RegionPlayerExtension(player);
    }

    public void Initialize() {}
}