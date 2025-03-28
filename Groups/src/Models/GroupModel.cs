using Microsoft.Xna.Framework;
using MongoDB.Bson.Serialization.Attributes;
using Amethyst.Permissions;
using Amethyst.Storages.Mongo;
using Amethyst.Network;

namespace Amethyst.Groups.Models;

[BsonIgnoreExtraElements]
public sealed class GroupModel : DataModel
{
    public GroupModel(string name) : base(name)
    {
        Permissions = new List<string>();
    }

    public List<string> Permissions;
    public string? Prefix;
    public string? Suffix;
    public NetColor? Color;
    public string? Parent;
    public bool IsDefault;
    public bool BlockTempGroup;

    private GroupModel? _parent;
    private DateTime _lastUpdateParent;

    internal void InsertParentPermissions(ref List<string> perms)
    {
        var parent = GetParent();
        if (parent == null) return;

        perms.InsertRange(0, parent.Permissions);
        parent.InsertParentPermissions(ref perms);
    }

    public GroupModel? GetParent()
    {
        if (Parent == null) 
            return null;

        _parent = GroupsModule._cachedModels.Find(p => p.Name == Parent);
        return _parent;
    }

    public override void Save()
    {
        GroupsModule.Groups.Save(this);
        GroupsModule.Reload();
    }

    public override void Remove()
    {
        GroupsModule.Groups.Remove(Name);
        GroupsModule.Reload();
    }
}
