using Amethyst.Storages.Mongo;
using MongoDB.Bson.Serialization.Attributes;

namespace Groups.Models;

[BsonIgnoreExtraElements]
public sealed class GroupUserModel(string name) : DataModel(name)
{
    public string? Group;
    public TempGroup? TempGroup;
    public List<string> PersonalPermissions = [];

    public override void Save()
    {
        Groups.Users.Save(this);
        Groups.RefreshUser(Name);
    }

    public override void Remove()
    {
        Groups.Users.Remove(Name);
        Groups.RefreshUser(Name);
    }
}
