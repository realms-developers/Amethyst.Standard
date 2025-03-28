using Amethyst.Core;
using Amethyst.Extensions.Modules;
using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Amethyst.Talk.Chat;
using Amethyst.Talk.Filtering;
using Amethyst.Talk.Rendering;

namespace Amethyst.Talk;

[AmethystModule("Amethyst.Talk", null)]
public static class GroupsModule
{
    public static IChatProvider? CustomProvider;
    private static IChatProvider _BasicProvider = new BasicChatProvider();

    public static IChatProvider Provider => CustomProvider ?? _BasicProvider;

    private static bool IsInitialized;
    [ModuleInitialize]
    public static void Initialize()
    {
        if (IsInitialized) return; IsInitialized = true;

        AmethystHooks.Players.OnPlayerGreet += static (plr) => Provider.HandleJoin(plr);
        AmethystHooks.Players.OnPlayerDisconnected += static (plr) => Provider.HandleLeave(plr);

        NetworkManager.Binding.AddInModule(ModuleTypes.NetText, OnPlayerText);
    }

    private static void OnPlayerText(in IncomingModule packet, PacketHandleResult result)
    {
        if (!packet.Player.IsCapable)
        {
            result.Ignore(Localization.Get("network.playerIsNotCapable", packet.Player.Language));
            return;
        }

        var reader = packet.GetReader();

        string command = reader.ReadString();
        string text = reader.ReadString();
        if (command != "Say") return;

        if (!ChatFilter.FilterText(packet.Player, text))
        {
            RenderResult renderResult = ChatRenderer.RenderMessage(packet.Player, text);
            Provider.HandlePlayerMessage(renderResult);
            result.Ignore(Localization.Get("talk.replacedWithCustom", packet.Player.Language));
        }
    }
}
