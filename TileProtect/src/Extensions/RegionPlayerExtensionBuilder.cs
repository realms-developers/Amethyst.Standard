using Amethyst.Players;
using Amethyst.Players.Extensions;

namespace TileProtect.Extensions;

public sealed class RegionPlayerExtensionBuilder : IPlayerExtensionBuilder<RegionPlayerExtension>
{
    public RegionPlayerExtension Build(NetPlayer player) => new(player);

    public void Initialize() { }
}
