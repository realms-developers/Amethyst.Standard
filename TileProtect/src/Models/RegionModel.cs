using Amethyst.Storages.Mongo;
using Microsoft.Xna.Framework;
using MongoDB.Bson.Serialization.Attributes;

namespace TileProtect.Models;

[BsonIgnoreExtraElements]
public sealed class RegionModel(string name) : DataModel(name)
{
    public int X;
    public int Y;
    public int X2;
    public int Y2;

    public int Z;

    /// <summary>
    /// Protection from all users (include owner).
    /// </summary>
    public bool[] SuperProtection = new bool[6];

    /// <summary>
    /// Only members can do actions.
    /// </summary>
    public bool[] Protection =
        [
            true, true, true, true, true, true
        ];

    public List<string> Members = [];

    public bool NoItems;
    public bool NoEnemies;
    public bool AutoGodMode;

    public List<string> EnterMessages = [];
    public List<RegionCommand> EnterCommands = [];

    public List<string> LeaveMessages = [];
    public List<RegionCommand> LeaveCommands = [];

    public string? Owner;

    public bool IsMember(string name) => Owner == name || Members.Contains(name);

    public Rectangle GetRectangle()
    {
        int x = Math.Min(X, X2);
        int y = Math.Min(Y, Y2);
        int x2 = Math.Max(X, X2);
        int y2 = Math.Max(Y, Y2);

        return new Rectangle(x, y, x2 - x, y2 - y);
    }

    public override void Save()
    {
        global::TileProtect.TileProtect.Regions.Save(this);
        global::TileProtect.TileProtect._cachedRegions = [.. global::TileProtect.TileProtect.Regions.FindAll()];
    }

    public override void Remove()
    {
        global::TileProtect.TileProtect.Regions.Remove(Name);
        global::TileProtect.TileProtect._cachedRegions = [.. global::TileProtect.TileProtect.Regions.FindAll()];
    }
}
