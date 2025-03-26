using Amethyst.Permissions;
using Amethyst.Players;

namespace Amethyst.Groups.Extensions;

public sealed class PermissionWorker : IPermissionWorker<NetPlayer>
{
    public PermissionAccess HasPermission(NetPlayer target, string permission)
    {
        return target.GetExtension<UserExtension>()!.HasPermission(permission);
    }

    public PermissionAccess HasChestPermission(NetPlayer target, int x, int y)
    {
        return target.GetExtension<UserExtension>()!.HasPermission("world.edit.chests");
    }
    
    public PermissionAccess HasSignPermission(NetPlayer target, int x, int y)
    {
        return target.GetExtension<UserExtension>()!.HasPermission("world.edit.signs");
    }

    public PermissionAccess HasTEPermission(NetPlayer target, int x, int y)
    {
        return target.GetExtension<UserExtension>()!.HasPermission("world.edit.tileentities");
    }

    public PermissionAccess HasTilePermission(NetPlayer target, int x, int y, int? width = null, int? height = null)
    {
        return target.GetExtension<UserExtension>()!.HasPermission("world.edit.tiles");
    }

    public PermissionAccess HasChestEditPermission(NetPlayer target, int x, int y)
    {
        return target.GetExtension<UserExtension>()!.HasPermission("world.edit.chests");
    }

    public PermissionAccess HasSignEditPermission(NetPlayer target, int x, int y)
    {
        return target.GetExtension<UserExtension>()!.HasPermission("world.edit.chests");
    }
}
