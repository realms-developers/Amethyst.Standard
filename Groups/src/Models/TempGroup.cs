using MongoDB.Bson.Serialization.Attributes;

namespace Groups.Models;

public class TempGroup(string group, DateTime time)
{
    [BsonIgnore]
    public GroupModel? GroupModel;
    public string Group = group;
    public DateTime Time = time;

    public GroupModel? GetGroup()
    {
        if (Time < DateTime.UtcNow)
        {
            return null;
        }

        GroupModel? group = Groups._cachedModels.Find(p => p.Name == Group && p.BlockTempGroup == false);

        return group;
    }
}
