using Amethyst.Storages.Config;

namespace WorldRegeneration;

public sealed class RegenConfiguration
{
    public static Configuration<RegenConfiguration> Config { get; } = new Configuration<RegenConfiguration>("WorldRegeneration", new RegenConfiguration());
    public static RegenConfiguration Instance => Config.Data;

    public bool AutoRegenerate { get; set; } = true;
    public int AutoRegenerateMinutes { get; set; } = 60;

    public string AutoRegenerateRootPath { get; set; } = "data/profiles/%PROFILE%";
    public string AutoRegenerateFileName { get; set; } = "%ROOT_PATH%/Auto_%WORLD_ID%.rwld";
}