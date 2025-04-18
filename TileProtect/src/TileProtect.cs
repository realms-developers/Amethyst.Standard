using Amethyst.Core;
using Amethyst.Extensions.Modules;
using Amethyst.Players.Extensions;
using Amethyst.Storages.Mongo;
using TileProtect.Extensions;
using TileProtect.Models;
using TileProtect.Network;

namespace TileProtect;

[AmethystModule(nameof(TileProtect))]
public static class TileProtect
{
    public static MongoModels<RegionModel> Regions { get; } = MongoDatabase.Main.Get<RegionModel>();

    public static IReadOnlyList<RegionModel> CachedRegions => _cachedRegions.AsReadOnly();

    internal static List<RegionModel> _cachedRegions = [.. Regions.FindAll()];

    private static bool _isInitialized;

    [ModuleInitialize]
    public static void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        RegionNetworking.Initialize();
        AmethystSession.PlayerPermissions.Register(new RegionPermissionsWorker());
        PlayerExtensions.RegisterBuilder(new RegionPlayerExtensionBuilder());
    }
}
