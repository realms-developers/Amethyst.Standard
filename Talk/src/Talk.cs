using Amethyst;
using Amethyst.Core;
using Amethyst.Extensions.Modules;
using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Talk.Filtering;
using Talk.Rendering;

namespace Talk;

[AmethystModule(nameof(Talk), null)]
public static class Talk
{
    public static IChatProvider? CustomProvider { get; set; }
    private static readonly IChatProvider _basicProvider = new BasicChatProvider();

    public static IChatProvider Provider => CustomProvider ?? _basicProvider;

    private static bool _isInitialized;

    [ModuleInitialize]
    public static void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

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

        BinaryReader reader = packet.GetReader();

        string command = reader.ReadString();
        string text = reader.ReadString();
        if (command != "Say")
        {
            return;
        }

        if (!ChatFilter.FilterText(packet.Player, text))
        {
            RenderResult renderResult = ChatRenderer.RenderMessage(packet.Player, text);
            Provider.HandlePlayerMessage(renderResult);
            result.Ignore(Localization.Get("talk.replacedWithCustom", packet.Player.Language));
        }
    }
}
