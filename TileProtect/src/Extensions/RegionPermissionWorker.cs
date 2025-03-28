using Amethyst.Permissions;
using Amethyst.Players;
using Amethyst.TileProtect.Models;

namespace Amethyst.TileProtect.Extensions;

public sealed class RegionPermissionsWorker : IPermissionWorker<NetPlayer>
{
    private PermissionAccess CheckPermission(int x, int y, NetPlayer player, ProtectType protectType)
    {
        foreach (var region in RegionUtils.FindRegions(x, y))
        {
            if (region.SuperProtection[(int)protectType]) return PermissionAccess.Blocked;

            if (region.Protection[(int)protectType] == true && region.IsMember(player.Name) == false)
                return PermissionAccess.Blocked;
        }
        
        return PermissionAccess.HasPermission;
    }

    public PermissionAccess HasPermission(NetPlayer target, string permission)
    {
        return PermissionAccess.None;
    }

    public PermissionAccess HasChestPermission(NetPlayer target, int x, int y)
    {
        return CheckPermission(x, y, target, ProtectType.OpenChests);
    }

    public PermissionAccess HasChestEditPermission(NetPlayer target, int x, int y)
    {
        return CheckPermission(x, y, target, ProtectType.EditChests);
    }

    public PermissionAccess HasSignPermission(NetPlayer target, int x, int y)
    {
        return CheckPermission(x, y, target, ProtectType.OpenSigns);
    }

    public PermissionAccess HasSignEditPermission(NetPlayer target, int x, int y)
    {
        return CheckPermission(x, y, target, ProtectType.EditSigns);
    }

    public PermissionAccess HasTEPermission(NetPlayer target, int x, int y)
    {
        return CheckPermission(x, y, target, ProtectType.Entities);
    }

    public PermissionAccess HasTilePermission(NetPlayer target, int x, int y, int? width = null, int? height = null)
    {
        return CheckPermission(x, y, target, ProtectType.Tiles);
    }
}
