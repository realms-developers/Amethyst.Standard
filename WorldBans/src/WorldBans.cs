using Amethyst.Core;
using Amethyst.Extensions.Plugins;
using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Amethyst.Players;
using Amethyst.Storages.SQL;
using Microsoft.Xna.Framework;
using WorldBans.Storages;

namespace WorldBans;

public sealed class WorldBans : PluginInstance
{
    public static SQLiteProvider SQLiteProvider
    {
        get;
        private set;
    } = null!;

    public override string Name => "WorldBans";

    public override Version Version => new(1, 0);

    protected override void Load()
    {
        SQLiteProvider = new(AmethystSession.Profile, $"{AmethystSession.Profile.Name}.{Name}");

        SQLiteProvider.OpenConnection();

        SQLiteProvider.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS ProjectileBans (
            ProjectileID INTEGER PRIMARY KEY
            )");

        NetworkManager.Binding.AddInPacket(PacketTypes.ProjectileNew, OnProjectileNew);
    }

    protected override void Unload()
    {
        SQLiteProvider.CloseConnection();

        SQLiteProvider.Dispose();

        SQLiteProvider = null!;

        NetworkManager.Binding.RemoveInPacket(PacketTypes.ProjectileNew, OnProjectileNew);
    }

    private void OnProjectileNew(in IncomingPacket packet, PacketHandleResult result)
    {
        NetPlayer target = packet.Player;

        if (target?.Socket == null || target.HasPermission("worldbans.bypass.proj"))
        {
            AmethystLog.Main.Debug(nameof(OnProjectileNew), "Skipped. Socket is null or player has bypass permissions.");

            return;
        }

        using BinaryReader reader = packet.GetReader();

        short index = reader.ReadInt16();
        Vector2 pos = NetExtensions.ReadVector2(reader);
        Vector2 vel = NetExtensions.ReadVector2(reader);
        byte owner = reader.ReadByte();
        short type = reader.ReadInt16();

        if (!ProjectileBan.IsBanned(type))
        {
            AmethystLog.Main.Debug(nameof(OnProjectileNew), $"Skipped. Projectile {type} is not banned.");

            return;
        }

        result.Ignore("worldbans.banned");

        using PacketWriter writer = new();

        byte[] packetBytes = writer
            .SetType((short)PacketTypes.ProjectileNew)
            .PackInt16(index)
            .PackSingle(-1)
            .PackSingle(-1)
            .PackSingle(0)
            .PackSingle(0)
            .PackSingle(0)
            .PackInt16(0)
            .PackByte((byte)target.Index)
            .PackInt16(0)
            .PackSingle(0)
            .PackSingle(0)
            .BuildPacket();

        target.Socket.SendPacket(packetBytes);
    }
}
