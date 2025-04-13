using static WorldBans.WorldBans;

namespace WorldBans.Storages;

internal class ProjectileBan : BaseModel
{
    public static ProjectileBan[] Collection
    {
        get
        {
            List<Dictionary<string, object>> queryResult = SQLiteProvider.ExecuteQuery("SELECT * FROM ProjectileBans");
            List<ProjectileBan> bans = [];

            foreach (Dictionary<string, object> row in queryResult)
            {
                bans.Add(new ProjectileBan
                {
                    ProjectileID = Convert.ToInt32(row["ProjectileID"])
                });
            }

            return [.. bans];
        }
    }

    public int ProjectileID { get; set; }

    public static bool IsBanned(int projectileId)
    {
        object? result = SQLiteProvider.ExecuteScalar(
            "SELECT 1 FROM ProjectileBans WHERE ProjectileID = @id",
            new Dictionary<string, object> { { "@id", projectileId } }
        );

        return result != null;
    }

    public static ProjectileBan? GetByProjectileId(int projectileId)
    {
        List<Dictionary<string, object>> queryResult = SQLiteProvider.ExecuteQuery(
            "SELECT * FROM ProjectileBans WHERE ProjectileID = @id",
            new Dictionary<string, object> { { "@id", projectileId } }
        );

        return queryResult.Count > 0
            ? new ProjectileBan
            {
                ProjectileID = Convert.ToInt32(queryResult[0]["ProjectileID"])
            }
            : null;
    }

    public override void Save()
    {
        if (!IsBanned(ProjectileID)) // New ban
        {
            SQLiteProvider.ExecuteNonQuery(
                "INSERT INTO ProjectileBans (ProjectileID) VALUES (@id)",
                new Dictionary<string, object> { { "@id", ProjectileID } });
        }
        // No update needed since we're only storing ProjectileID
    }

    public override void Delete()
    {
        SQLiteProvider.ExecuteNonQuery(
            "DELETE FROM ProjectileBans WHERE ProjectileID = @id",
            new Dictionary<string, object> { { "@id", ProjectileID } }
        );
    }
}
