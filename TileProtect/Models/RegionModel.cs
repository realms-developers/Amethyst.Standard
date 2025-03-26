using Amethyst.Storages.Mongo;
using Amethyst.TileProtect.Models;
using Microsoft.Xna.Framework;
using MongoDB.Bson.Serialization.Attributes;

namespace Amethyst.TileProtect;

[BsonIgnoreExtraElements]
public sealed class RegionModel : DataModel
{
    public RegionModel(string name) : base(name)
    {
        Members = new List<string>();

        SuperProtection = new bool[6];
        Protection = new bool[6]
        {
            true, true, true, true, true, true
        };

        EnterCommands = new List<RegionCommand>();
        EnterMessages = new List<string>();

        LeaveCommands = new List<RegionCommand>();
        LeaveMessages = new List<string>();
    }

    public int X;
    public int Y;
    public int X2;
    public int Y2;

    public int Z;

    /// <summary>
    /// Protection from all users (include owner).
    /// </summary>
    public bool[] SuperProtection;

    /// <summary>
    /// Only members can do actions.
    /// </summary>
    public bool[] Protection;

    public List<string> Members;

    public bool NoItems;
    public bool NoEnemies;
    public bool AutoGodMode;

    public List<string> EnterMessages;
    public List<RegionCommand> EnterCommands;

    public List<string> LeaveMessages;
    public List<RegionCommand> LeaveCommands;

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
        ProtectionModule.Regions.Save(this);
        ProtectionModule._cachedRegions = ProtectionModule.Regions.FindAll().ToList();
    }

    public override void Remove()
    {
        ProtectionModule.Regions.Remove(Name);
        ProtectionModule._cachedRegions = ProtectionModule.Regions.FindAll().ToList();
    }
}
