using MongoDB.Bson.Serialization.Attributes;

namespace Amethyst.Groups.Models;

public class TempGroup
{
    public TempGroup(string group, DateTime time)
    {
        Group = group;
        Time = time;
    }

    [BsonIgnore]
    public GroupModel? GroupModel;
    public string Group;
    public DateTime Time;

    public GroupModel? GetGroup()
    {
        if (Time < DateTime.UtcNow) return null;

        var group = GroupsModule._cachedModels.Find(p => p.Name == Group && p.BlockTempGroup == false);

        return group;
    }
}