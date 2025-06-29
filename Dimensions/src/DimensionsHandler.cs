using Amethyst;
using Amethyst.Network;
using Amethyst.Network.Handling;
using Amethyst.Network.Handling.Base;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Network.Utilities;
using Amethyst.Server.Entities.Players;
using MongoDB.Driver.Linq;
using Terraria;

namespace Dimensions;

public sealed class DimensionsHandler : INetworkHandler
{
    public string Name => "net.dimensions";

    public void Load()
    {
        NetworkManager.AddDirectHandler(67, OnDimensionsPacket);
        NetworkManager.SetMainHandler<PlayerRequestWorldInfo>(OnRequestWorldInfo);
    }

    private void OnRequestWorldInfo(PlayerEntity plr, ref PlayerRequestWorldInfo packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.WaitingWorldInfoRequest)
        {
            return;
        }

        plr.SendPacketBytes(PacketSendingUtility.CreateWorldInfoPacket());

        Main.SyncAnInvasion(plr.Index);

        PacketSendingUtility.SendFullWorld(plr, -1, -1);

        plr.SendPacketBytes(PlayerFinishedConnectionPacket.Serialize(new()));
        plr.SetGodMode(false);
        plr.Phase = ConnectionPhase.WaitingPlayerSpawn;
    }

    private void OnDimensionsPacket(PlayerEntity plr, ReadOnlySpan<byte> data, ref bool ignore)
    {
        FastPacketReader reader = new(data, 3);
        if (reader.ReadInt16() == 0)
        {
            string ip = reader.ReadString();
            plr.IP = ip;

            AmethystLog.Network.Info("Dimensions", $"Player {plr.Name} ({plr.Index}) connected with IP: {ip}");
        }
    }

    public void Unload()
    {
        HandshakeHandler handler = (HandshakeHandler)HandlerManager.GetHandlers().First(p => p.Name == "net.amethyst.HandshakeHandler");
        NetworkManager.SetMainHandler<PlayerRequestWorldInfo>(handler.OnPlayerRequestWorldInfo);
        NetworkManager.RemoveDirectHandler(67, OnDimensionsPacket);
    }
}
