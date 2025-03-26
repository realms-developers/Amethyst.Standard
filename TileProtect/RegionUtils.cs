using System.Collections.Generic;

namespace Amethyst.TileProtect;

public static class RegionUtils
{
    public static IEnumerable<RegionModel> FindRegions(int x, int y)
    {
        var regions = ProtectionModule._cachedRegions;
        foreach (var region in regions)
        {
            if (region.GetRectangle().Contains(x, y))
                yield return region;
        }
    }
}