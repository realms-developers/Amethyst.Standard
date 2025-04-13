using static WorldBans.WorldBans;

namespace WorldBans.Storages;

internal class ItemBan : BaseModel
{
    public static ItemBan[] Collection
    {
        get
        {
            List<Dictionary<string, object>> queryResult = SQLiteProvider.ExecuteQuery("SELECT * FROM ItemBans");
            List<ItemBan> bans = [];

            foreach (Dictionary<string, object> row in queryResult)
            {
                bans.Add(new ItemBan
                {
                    ItemID = Convert.ToInt32(row["ItemID"])
                });
            }

            return [.. bans];
        }
    }

    public int ItemID { get; set; }

    public static bool IsBanned(int itemId)
    {
        object? result = SQLiteProvider.ExecuteScalar(
            "SELECT 1 FROM ItemBans WHERE ItemID = @id",
            new Dictionary<string, object> { { "@id", itemId } }
        );

        return result != null;
    }

    public static ItemBan? GetByItemId(int itemId)
    {
        List<Dictionary<string, object>> queryResult = SQLiteProvider.ExecuteQuery(
            "SELECT * FROM ItemBans WHERE ItemID = @id",
            new Dictionary<string, object> { { "@id", itemId } }
        );

        return queryResult.Count > 0
            ? new ItemBan
            {
                ItemID = Convert.ToInt32(queryResult[0]["ItemID"])
            }
            : null;
    }

    public override void Save()
    {
        if (!IsBanned(ItemID)) // New ban
        {
            SQLiteProvider.ExecuteNonQuery(
                "INSERT INTO ItemBans (ItemID) VALUES (@id)",
                new Dictionary<string, object> { { "@id", ItemID } });
        }
        // No update needed since we're only storing ItemID
    }

    public override void Delete()
    {
        SQLiteProvider.ExecuteNonQuery(
            "DELETE FROM ItemBans WHERE ItemID = @id",
            new Dictionary<string, object> { { "@id", ItemID } }
        );
    }
}
