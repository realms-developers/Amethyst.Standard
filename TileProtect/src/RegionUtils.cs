using TileProtect.Models;

namespace TileProtect;

public static class RegionUtils
{
    public static IEnumerable<RegionModel> FindRegions(int x, int y)
    {
        List<RegionModel> regions = ProtectionModule._cachedRegions;
        foreach (RegionModel region in regions)
        {
            if (region.GetRectangle().Contains(x, y))
            {
                yield return region;
            }
        }
    }
}
