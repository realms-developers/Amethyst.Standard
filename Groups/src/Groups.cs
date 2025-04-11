using Amethyst.Core;
using Amethyst.Extensions.Modules;
using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Amethyst.Players;
using Amethyst.Players.Extensions;
using Amethyst.Storages.Mongo;
using Groups.Extensions;
using Groups.Models;
using Talk.Rendering;

namespace Groups;

[AmethystModule("Groups", null)]
public static class Groups
{
    public static MongoModels<GroupModel> GroupModels { get; } = MongoDatabase.Main.Get<GroupModel>();
    public static MongoModels<GroupUserModel> Users { get; } = MongoDatabase.Main.Get<GroupUserModel>();

    public static IReadOnlyList<GroupModel> CachedGroups => _cachedModels.AsReadOnly();

    internal static List<GroupModel> _cachedModels = [.. GroupModels.FindAll()];

    private static bool _isInitialized;
    [ModuleInitialize]
    public static void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        PlayerExtensions.RegisterBuilder(new UserExtensionBuilder());
        AmethystSession.PlayerPermissions.Register(new PermissionWorker());

        NetworkManager.Binding.AddInPacket(PacketTypes.RequestWorldInfo, OnRequestWorldInfo);

        ChatRenderer.AddRenderer(RenderGroups);
    }

    private static void RenderGroups(RenderOperation operation)
    {
        UserExtension? ext = operation.Player.GetExtension<UserExtension>();

        if (ext?.Group == null)
        {
            return;
        }

        GroupModel group = ext.Group;
        operation.Color = group.Color ?? operation.Color;

        if (group.Prefix != null)
        {
            operation.Prefix.Add(new RenderSnippet(RenderPriority.PostHigh, group.Prefix));
        }

        if (group.Suffix != null)
        {
            operation.Suffix.Add(new RenderSnippet(RenderPriority.PostHigh, group.Suffix));
        }
    }

    private static void OnRequestWorldInfo(in IncomingPacket packet, PacketHandleResult result)
    {
        try
        {
            UserExtension? ext = packet.Player.GetExtension<UserExtension>();
            ext!.Refresh();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public static void Reload()
    {
        _cachedModels = [.. GroupModels.FindAll()];
        RefreshUsers();
    }

    public static void RefreshUser(string name)
    {
        foreach (NetPlayer plr in PlayerManager.Tracker)
        {
            if (plr.Name == name)
            {
                plr.GetExtension<UserExtension>()!.Refresh();
            }
        }
    }

    public static void RefreshUsers()
    {
        foreach (NetPlayer plr in PlayerManager.Tracker)
        {
            plr.GetExtension<UserExtension>()!.Refresh();
        }
    }
}
