using Amethyst.Core;
using Amethyst.Extensions.Modules;
using Amethyst.Groups.Extensions;
using Amethyst.Groups.Models;
using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Amethyst.Players;
using Amethyst.Players.Extensions;
using Amethyst.Storages.Mongo;
using Amethyst.Talk.Rendering;

namespace Amethyst.Groups;

[AmethystModule("Amethyst.Groups", null)]
public static class GroupsModule
{
    public static MongoModels<GroupModel> Groups { get; } = MongoDatabase.Main.Get<GroupModel>();
    public static MongoModels<GroupUserModel> Users { get; } = MongoDatabase.Main.Get<GroupUserModel>();

    public static IReadOnlyList<GroupModel> CachedGroups => _cachedModels.AsReadOnly();

    internal static List<GroupModel> _cachedModels = Groups.FindAll().ToList();

    private static bool IsInitialized;
    [ModuleInitialize]
    public static void Initialize()
    {
        if (IsInitialized) return; IsInitialized = true;

        PlayerExtensions.RegisterBuilder(new UserExtensionBuilder());
        AmethystSession.PlayerPermissions.Register(new PermissionWorker());

        NetworkManager.Binding.AddInPacket(PacketTypes.RequestWorldInfo, OnRequestWorldInfo);

        ChatRenderer.AddRenderer(RenderGroups);
    }

    private static void RenderGroups(RenderOperation operation)
    {
        var ext = operation.Player.GetExtension<UserExtension>();
        
        if (ext?.Group == null) return;

        var group = ext.Group;
        operation.Color = group.Color ?? operation.Color;

        if (group.Prefix != null)
            operation.Prefix.Add(new RenderSnippet(RenderPriority.PostHigh, group.Prefix));

        if (group.Suffix != null)
            operation.Suffix.Add(new RenderSnippet(RenderPriority.PostHigh, group.Suffix));
    }

    private static void OnRequestWorldInfo(in IncomingPacket packet, PacketHandleResult result)
    {
        try
        {
            var ext = packet.Player.GetExtension<UserExtension>();
            ext!.Refresh();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public static void Reload()
    {
        _cachedModels = Groups.FindAll().ToList();
        RefreshUsers();
    }

    public static void RefreshUser(string name)
    {
        foreach (var plr in PlayerManager.Tracker)
            if (plr.Name == name)
                plr.GetExtension<UserExtension>()!.Refresh();
    }

    public static void RefreshUsers()
    {
        foreach (var plr in PlayerManager.Tracker)
            plr.GetExtension<UserExtension>()!.Refresh();
    }
}