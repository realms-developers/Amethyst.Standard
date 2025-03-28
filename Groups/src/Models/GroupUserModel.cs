using Amethyst.Storages.Mongo;
using MongoDB.Bson.Serialization.Attributes;

namespace Amethyst.Groups.Models;

[BsonIgnoreExtraElements]
public sealed class GroupUserModel : DataModel
{
    public GroupUserModel(string name) : base(name)
    {
        PersonalPermissions = new List<string>();
    }

    public string? Group;
    public TempGroup? TempGroup;
    public List<string> PersonalPermissions;

    public override void Save()
    {
        GroupsModule.Users.Save(this);
        GroupsModule.RefreshUser(Name);
    }

    public override void Remove()
    {
        GroupsModule.Users.Remove(Name);
        GroupsModule.RefreshUser(Name);
    }
}
