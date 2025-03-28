using Amethyst.Core;
using Amethyst.Extensions.Modules;
using Amethyst.Players.Extensions;
using Amethyst.Storages.Mongo;
using Amethyst.TileProtect.Extensions;
using Amethyst.TileProtect.Network;

namespace Amethyst.TileProtect;

[AmethystModule("Amethyst.TileProtect", null)]
public static class ProtectionModule
{
    public static MongoModels<RegionModel> Regions { get; } = MongoDatabase.Main.Get<RegionModel>();

    public static IReadOnlyList<RegionModel> CachedRegions => _cachedRegions.AsReadOnly();

    internal static List<RegionModel> _cachedRegions = Regions.FindAll().ToList();

    private static bool IsInitialized;
    [ModuleInitialize]
    public static void Initialize()
    {
        if (IsInitialized) return; IsInitialized = true;

        RegionNetworking.Initialize();
        AmethystSession.PlayerPermissions.Register(new RegionPermissionsWorker());
        PlayerExtensions.RegisterBuilder(new RegionPlayerExtensionBuilder());
    }
}
