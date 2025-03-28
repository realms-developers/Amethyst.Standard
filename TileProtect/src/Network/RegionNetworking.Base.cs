using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Amethyst.Players;
using Amethyst.TileProtect.Extensions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Amethyst.TileProtect.Network;

public static partial class RegionNetworking
{
    internal static void Initialize()
    {
        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.TileInteract, HandlePlayerPoint);
        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.PlayerUpdate, HandlePlayerRegionZone);

        // Tiles
        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.TileInteract, OnTileInteract);

        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.DoorUse, OnDoorUse);

        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.LiquidSet, OnLiquidSet);

        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.ChestUnlock, OnChestUnlock);

        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.HitSwitch, OnHitSwitch);

        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.PaintTile, OnPaintTile);

        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.PaintWall, OnPaintWall);

        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.PlaceObject, OnPlaceObject);

        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.TileSendSquare, OnTileSendSquare);

        // Chests
        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.ChestGetContents, OnChestGetContents);

        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.PlaceChest, OnPlaceChest);

        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.ChestOpen, OnChestOpen);

        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.ChestItem, OnEditChestItem);

        // Signs
        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.SignNew, OnSignInteract);

        // Tile Entities
        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.PlaceTileEntity, OnPlaceTileEntity);

        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.UpdateTileEntity, OnUpdateTileEntity);

        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.PlaceItemFrame, OnPlaceItemFrame);

        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.GemLockToggle, OnGemLockToggle);

        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.TileEntityDisplayDollItemSync, OnUpdateTileEntityItems);
        NetworkManager.Binding.AddInPacket(Amethyst.Network.PacketTypes.TileEntityHatRackItemSync, OnUpdateTileEntityItems);
    }

    private static void OnTileInteract(in IncomingPacket packet, PacketHandleResult result)
        => OffsetRead(in packet, result, 1, (plr, x, y, w, h) => !plr.HasTilePermission(x, y), false);

    private static void OnDoorUse(in IncomingPacket packet, PacketHandleResult result)
        => OffsetRead(in packet, result, 1, (plr, x, y, w, h) => !plr.HasTilePermission(x, y), false);

    private static void OnLiquidSet(in IncomingPacket packet, PacketHandleResult result)
        => OffsetRead(in packet, result, 0, (plr, x, y, w, h) => !plr.HasTilePermission(x, y), false);

    private static void OnChestUnlock(in IncomingPacket packet, PacketHandleResult result)
        => OffsetRead(in packet, result, 1, (plr, x, y, w, h) => !plr.HasTilePermission(x, y) || !plr.HasChestEditPermission(x, y), false);

    private static void OnHitSwitch(in IncomingPacket packet, PacketHandleResult result)
        => OffsetRead(in packet, result, 0, (plr, x, y, w, h) => !plr.HasTilePermission(x, y), false);

    private static void OnPaintTile(in IncomingPacket packet, PacketHandleResult result)
        => OffsetRead(in packet, result, 0, (plr, x, y, w, h) => !plr.HasTilePermission(x, y), false);

    private static void OnPaintWall(in IncomingPacket packet, PacketHandleResult result)
        => OffsetRead(in packet, result, 0, (plr, x, y, w, h) => !plr.HasTilePermission(x, y), false);

    private static void OnPlaceObject(in IncomingPacket packet, PacketHandleResult result)
        => OffsetRead(in packet, result, 79, (plr, x, y, w, h) => !plr.HasTilePermission(x, y), false);

    private static void OnTileSendSquare(in IncomingPacket packet, PacketHandleResult result)
        => OffsetRead(in packet, result, 1, (plr, x, y, w, h) => !plr.HasTilePermission(x, y), true);

    private static void OnChestGetContents(in IncomingPacket packet, PacketHandleResult result)
        => OffsetRead(in packet, result, 1, (plr, x, y, w, h) => !plr.HasChestPermission(x, y), false);

    private static void OnPlaceChest(in IncomingPacket packet, PacketHandleResult result)
        => OffsetRead(in packet, result, 1, (plr, x, y, w, h) => !plr.HasChestPermission(x, y), false);

    private static void OnPlaceTileEntity(in IncomingPacket packet, PacketHandleResult result)
        => OffsetRead(in packet, result, 0, (plr, x, y, w, h) => !plr.HasTEPermission(x, y), true);

    private static void OnPlaceItemFrame(in IncomingPacket packet, PacketHandleResult result)
        => OffsetRead(in packet, result, 0, (plr, x, y, w, h) => !plr.HasTEPermission(x, y), true);

    private static void OnGemLockToggle(in IncomingPacket packet, PacketHandleResult result)
        => OffsetRead(in packet, result, 0, (plr, x, y, w, h) => !plr.HasTEPermission(x, y), true);

    private static void HandlePlayerRegionZone(in IncomingPacket packet, PacketHandleResult result)
    {
        if (!packet.Player.IsCapable)
        {
            return;
        }

        var ext = packet.Player.GetExtension<RegionPlayerExtension>();
        ext!.UpdateRegions();
    }

    private static void HandlePlayerPoint(in IncomingPacket packet, PacketHandleResult result)
    {
        if (!packet.Player.IsCapable)
        {
            result.Ignore("Player is not capable");
            return;
        }

        var ext = packet.Player.GetExtension<RegionPlayerExtension>();
        if (ext!.pointToDo == 0) return;

        var reader = packet.GetReader();
        reader.BaseStream.Position += 1;

        int x = reader.ReadInt16();
        int y = reader.ReadInt16();

        if (ext.pointToDo == 1) ext.point1 = new Point(x, y);
        else ext.point2 = new Point(x, y);

        ext.pointToDo = 0;

        packet.Player.ReplySuccess(Localization.Get("regions.pointSet", packet.Player.Language));
    }



    #region Region checks

    private static void OnUpdateTileEntityItems(in IncomingPacket packet, PacketHandleResult result)
    {
        if (!packet.Player.IsCapable)
        {
            result.Ignore("Player is not capable");
            return;
        }

        var reader = packet.GetReader();
        reader.ReadByte();
        int index = reader.ReadInt32();

        if (TileEntity.ByID.TryGetValue(index, out var entity))
        {
            if (!packet.Player.HasTEPermission(entity.Position.X, entity.Position.Y))
            {
                result.Ignore("Player does not have permission for interacting with TE");
                return;
            }
        }
    }

    private static void OnUpdateTileEntity(in IncomingPacket packet, PacketHandleResult result)
    {
        if (!packet.Player.IsCapable)
        {
            result.Ignore("Player is not capable");
            return;
        }

        var reader = packet.GetReader();
        int num67 = reader.ReadInt32();

        bool create = reader.ReadBoolean();

        if (create)
        {
            reader.ReadByte();
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();

            if (!packet.Player.HasTEPermission(x, y))
            {
                result.Ignore("Player does not have permission for interacting with TE");
                return;
            }
        }
        else
        {
            if (TileEntity.ByID.TryGetValue(num67, out var entity))
            {
                if (!packet.Player.HasTEPermission(entity.Position.X, entity.Position.Y))
                {
                    result.Ignore("Player does not have permission for interacting with TE");
                    return;
                }
            }
        }
    }

    private static void OnSignInteract(in IncomingPacket packet, PacketHandleResult result)
    {
        if (!packet.Player.IsCapable)
        {
            result.Ignore("Player is not capable");
            return;
        }

        var reader = packet.GetReader();

        int index = reader.ReadInt16();
        int x = reader.ReadInt16();
        int y = reader.ReadInt16();
        string text = reader.ReadString();
        int owner = reader.ReadByte();
        BitsByte bitsByte = reader.ReadByte();

        if (index < 0 || index >= 1000) return;

        var sign = Main.sign[index];

        if (sign != null && sign.x > 0 && sign.y > 0 && (sign.x != x || sign.y != y))
        {
            result.Ignore("Invalid sign interaction by XY");
            return;
        }

        if (!packet.Player.HasSignEditPermission(x, y) || (sign != null && !packet.Player.HasSignEditPermission(sign.x, sign.y)))
        {
            result.Ignore("Player does not have permission for interacting with sign");
            NetMessage.TrySendData(47, packet.Sender, -1, Terraria.Localization.NetworkText.Empty, index, owner);
            return;
        }

        var oldText = sign?.text ?? "";

        if (text.Length > 500)
        {
            result.Ignore("Invalid sign text length");
            return;
        }
    }

    private static void OnChestOpen(in IncomingPacket packet, PacketHandleResult result)
    {
        if (!packet.Player.IsCapable)
        {
            result.Ignore("Player is not capable");
            return;
        }

        var reader = packet.GetReader();
        int chestIndex = reader.ReadInt16();
        int x = reader.ReadInt16();
        int y = reader.ReadInt16();
        int editFlags = reader.ReadByte();

        if (chestIndex < 0 || chestIndex >= 8000) return;

        var chest = Main.chest[chestIndex];
        if (chest == null) return;

        if (editFlags == 1 && !packet.Player.HasChestEditPermission(chest.x, chest.y))
        {
            result.Ignore("Player does not have permission for interacting [ChestEdit, Rename]");
            return;
        }
        else if (editFlags == 0 && !packet.Player.HasChestPermission(chest.x, chest.y))
        {
            result.Ignore("Player does not have permission for interacting [ChestEdit, Sync]");
            return;
        }
    }

    private static void OnEditChestItem(in IncomingPacket packet, PacketHandleResult result)
    {
        if (!packet.Player.IsCapable)
        {
            result.Ignore("Player is not capable");
            return;
        }

        var reader = packet.GetReader();
        int chestIndex = reader.ReadInt16();
        int slot = reader.ReadByte();

        if (chestIndex < 0 || chestIndex >= 8000) return;

        var chest = Main.chest[chestIndex];
        if (chest == null) return;

        if (packet.Player.TPlayer.chest != chestIndex) return;

        if (slot >= chest.item.Length) return;

        if (!packet.Player.HasChestEditPermission(chest.x, chest.y))
        {
            result.Ignore("Player does not have permission for interacting [ChestEdit]");
            NetMessage.SendData(32, packet.Sender, -1, Terraria.Localization.NetworkText.Empty, chestIndex, slot);
        }
    }

    private static void OffsetRead(in IncomingPacket packet,
        PacketHandleResult result,
        int offset,
        Func<NetPlayer, int, int, int?, int?, bool> checkFunc,
        bool isRect,
        bool isWire = false,
        bool notify = true,
        string notifyMessage = "tileprotect.tileProtected",
        bool sendRect = true)
    {
        if (!packet.Player.IsCapable)
        {
            result.Ignore("Player is not capable");
            return;
        }

        var reader = packet.GetReader();
        reader.BaseStream.Position += offset;

        int x = reader.ReadInt16();
        int y = reader.ReadInt16();
        int? w = isRect ? reader.ReadByte() : isWire ? reader.ReadInt16() : null;
        int? h = isRect ? reader.ReadByte() : isWire ? reader.ReadInt16() : null;

        if (isWire ? checkFunc(packet.Player, Math.Min(x, w!.Value), Math.Min(y, h!.Value), Math.Max(x, w!.Value), Math.Max(y, h!.Value)) :
                     checkFunc(packet.Player, x, y, w, h))
        {
            result.Ignore("Player does not have permission for interacting [Basic]");

            var ext = packet.Player.GetExtension<RegionPlayerExtension>()!;
            if (notify && ext._notifyDelay == null || ext._notifyDelay < DateTime.UtcNow)
            {
                ext._notifyDelay = DateTime.UtcNow.AddSeconds(2);
                packet.Player.ReplyError(Localization.Get(notifyMessage, packet.Player.Language));
            }

            if (sendRect)
            {
                NetMessage.SendTileSquare(packet.Sender, x, y, 2, 2, Terraria.ID.TileChangeType.None);
            }
            return;
        }
    }

    #endregion
}
