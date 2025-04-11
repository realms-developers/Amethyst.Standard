using Amethyst.Network;
using Amethyst.Storages.Mongo;
using MongoDB.Bson.Serialization.Attributes;

namespace Groups.Models;

[BsonIgnoreExtraElements]
public sealed class GroupModel(string name) : DataModel(name)
{
    public List<string> Permissions = [];
    public string? Prefix;
    public string? Suffix;
    public NetColor? Color;
    public string? Parent;
    public bool IsDefault;
    public bool BlockTempGroup;

    private GroupModel? _parent;

    internal void InsertParentPermissions(ref List<string> perms)
    {
        GroupModel? parent = GetParent();
        if (parent == null)
        {
            return;
        }

        perms.InsertRange(0, parent.Permissions);
        parent.InsertParentPermissions(ref perms);
    }

    public GroupModel? GetParent()
    {
        if (Parent == null)
        {
            return null;
        }

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
