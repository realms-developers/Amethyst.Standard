using Amethyst.Kernel;
using Terraria;

namespace WorldRegeneration;

public static class RegenUtils
{
    public static string RootPath()
        => RegenConfiguration.Instance.AutoRegenerateRootPath
            .Replace("%PROFILE%", AmethystSession.Profile.Name);

    public static string GetDefaultWorldPath()
        => RegenConfiguration.Instance.AutoRegenerateRootPath
            .Replace("%ROOT_PATH%", RootPath())
            .Replace("%WORLD_ID%", Main.worldID.ToString());

    public static void SaveWorld(string fileName)
    {

    }

    public static bool LoadWorld(string fileName)
    {
        if (!File.Exists(fileName))
            return false;

        // Load the world data from the file
        return true;
    }
}