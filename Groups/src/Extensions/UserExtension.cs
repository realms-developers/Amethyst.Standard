using Amethyst.Permissions;
using Amethyst.Players;
using Amethyst.Players.Extensions;
using Groups.Models;

namespace Groups.Extensions;

public sealed class UserExtension : IPlayerExtension
{
    internal UserExtension(NetPlayer player)
    {
        Player = player;
        PersonalPermissions = [];

        _sharedPermissions = [];
        SharedPermissions = _sharedPermissions.AsReadOnly();
    }

    public NetPlayer Player { get; }

    public GroupModel? Group;
    public TempGroup? TempGroup;
    public List<string> PersonalPermissions;
    public IReadOnlyList<string> SharedPermissions;

    internal List<string> _sharedPermissions;

    public PermissionAccess HasPermission(string permission)
    {
        if (_sharedPermissions.Any(p => p.Equals("operator", StringComparison.OrdinalIgnoreCase) || p == "*"))
        {
            return PermissionAccess.HasPermission;
        }

        if (_sharedPermissions.Contains("!" + permission))
        {
            return PermissionAccess.Blocked;
        }

        string[] array = permission.Split('.');
        PermissionAccess partedAccess = HasPartedPermission(array);
        if (partedAccess == PermissionAccess.Blocked || partedAccess == PermissionAccess.HasPermission)
        {
            return partedAccess;
        }

        foreach (string perm in _sharedPermissions)
        {
            if (perm == permission)
            {
                return PermissionAccess.HasPermission;
            }
        }

        return PermissionAccess.None;
    }

    private PermissionAccess HasPartedPermission(string[] array)
    {
        foreach (string part in array)
        {
            if (_sharedPermissions.Contains("!" + part + ".*"))
            {
                return PermissionAccess.Blocked;
            }

            if (_sharedPermissions.Contains(part + ".*"))
            {
                return PermissionAccess.HasPermission;
            }
        }

        return PermissionAccess.None;
    }

    public GroupModel? GetGroup()
    {
        if (TempGroup != null && TempGroup.Time > DateTime.UtcNow)
        {
            return TempGroup.GroupModel;
        }

        return Group;
    }

    public void Load() { }

    public void Unload() { }

    public void Refresh()
    {
        if (Player.Name == "")
        {
            return;
        }

        GroupUserModel? usr = GroupsModule.Users.Find(Player.Name);
        if (usr == null)
        {
            usr = new GroupUserModel(Player.Name)
            {
                Group = GroupsModule.CachedGroups.FirstOrDefault(p => p.IsDefault)?.Name
            };
            usr.Save();
        }

        GroupModel? group = GroupsModule.CachedGroups.FirstOrDefault(p => p.Name == usr.Group) ?? null;

        PersonalPermissions = usr.PersonalPermissions ?? [];
        _sharedPermissions = PersonalPermissions;

        Group = group;
        TempGroup = usr.TempGroup;
        if (TempGroup != null)
        {
            TempGroup.GroupModel = GroupsModule.CachedGroups.FirstOrDefault(p => p.Name == TempGroup.Group) ?? null;

            if (TempGroup.GroupModel != null)
            {
                _sharedPermissions.AddRange(TempGroup.GroupModel.Permissions);
                TempGroup.GroupModel.InsertParentPermissions(ref _sharedPermissions);
            }
        }
        else if (Group != null)
        {
            _sharedPermissions.AddRange(Group.Permissions);
            Group.InsertParentPermissions(ref _sharedPermissions);
        }

        SharedPermissions = _sharedPermissions.AsReadOnly();
    }
}
