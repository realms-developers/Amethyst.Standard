using System.Drawing;
using System.IO.Compression;
using System.IO.Pipelines;
using Amethyst.Kernel;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Anvil.Regions.Data;
using Anvil.Regions.Data.Models;
using Terraria;

namespace WorldRegeneration;

public static class RegenUtils
{
    public static string RootPath()
        => RegenConfiguration.Instance.AutoRegenerateRootPath
            .Replace("%PROFILE%", AmethystSession.Profile.Name);

    public static string GetDefaultWorldPath()
        => RegenConfiguration.Instance.AutoRegenerateRootPath
            .Replace("%ROOT_PATH%", RootPath())
            .Replace("%WORLD_ID%", Main.worldID.ToString());

    public static unsafe void SaveWorld(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or whitespace.", nameof(fileName));

        if (File.Exists(fileName))
            File.Delete(fileName);

        using FileStream fileStream = new(fileName, FileMode.Create, FileAccess.Write);
        using DeflateStream stream = new(fileStream, CompressionLevel.Optimal);
        using BinaryWriter writer = new(stream);

        for (int i = 0; i < Main.maxTilesX; i++)
            for (int j = 0; j < Main.maxTilesY; j++)
            {
                TileData* ptr = Main.tile[i, j].ptr;

                writer.Write(ptr->type);
                writer.Write(ptr->wall);
                writer.Write(ptr->liquid);
                writer.Write(ptr->sTileHeader);
                writer.Write(ptr->bTileHeader);
                writer.Write(ptr->bTileHeader2);
                writer.Write(ptr->bTileHeader3);

                if (Main.tileFrameImportant[ptr->type])
                {
                    writer.Write(ptr->frameX);
                    writer.Write(ptr->frameY);
                }
            }

        List<Chest> chests = Main.chest.Where(c => c != null).ToList();
        writer.Write(chests.Count);
        foreach (Chest chest in chests)
        {
            if (chest == null) continue;

            writer.Write(chest.x);
            writer.Write(chest.y);
            writer.Write(chest.name);
            writer.Write(chest.frame);
            writer.Write(chest.frameCounter);
            for (int i = 0; i < chest.item.Length; i++)
            {
                Item item = chest.item[i];
                writer.Write(item.netID);
                writer.Write(item.stack);
                writer.Write(item.prefix);
            }
        }
    }

    public static unsafe bool LoadWorld(string fileName)
    {
        if (!File.Exists(fileName))
            return false;

        using FileStream fileStream = new(fileName, FileMode.Open, FileAccess.Read);
        using DeflateStream stream = new(fileStream, CompressionMode.Decompress);
        using BinaryReader reader = new(stream);

        Span<RegionBounds> regions = ModuleStorage.Regions.FindAll()
            .Select(r => new RegionBounds(r.X, r.Y, r.X2, r.Y2))
            .ToArray();

        bool InZone(ref Span<RegionBounds> regions, int x, int y)
        {
            foreach (RegionBounds region in regions)
            {
                if (x >= region.X && x < region.X2 && y >= region.Y && y < region.Y2)
                    return true;
            }
            return false;
        }

        for (int i = 0; i < Main.maxTilesX; i++)
        {
            for (int j = 0; j < Main.maxTilesY; j++)
            {
                if (InZone(ref regions, i, j))
                {
                    ushort type = reader.ReadUInt16();
                    reader.ReadUInt16();
                    reader.ReadByte();
                    reader.ReadUInt16();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();

                    if (Main.tileFrameImportant[type])
                    {
                        short frameX = reader.ReadInt16();
                        short frameY = reader.ReadInt16();
                    }
                    continue;
                }

                TileData* ptr = Main.tile[i, j].ptr;

                ptr->type = reader.ReadUInt16();
                ptr->wall = reader.ReadUInt16();
                ptr->liquid = reader.ReadByte();
                ptr->sTileHeader = reader.ReadUInt16();
                ptr->bTileHeader = reader.ReadByte();
                ptr->bTileHeader2 = reader.ReadByte();
                ptr->bTileHeader3 = reader.ReadByte();

                if (Main.tileFrameImportant[ptr->type])
                {
                    ptr->frameX = reader.ReadInt16();
                    ptr->frameY = reader.ReadInt16();
                }
            }
        }

        int chestCount = reader.ReadInt32();

        for (int i = 0; i < chestCount; i++)
        {
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            string name = reader.ReadString();
            int frame = reader.ReadInt32();
            int frameCounter = reader.ReadInt32();

            Chest chest = new()
            {
                x = x,
                y = y,
                name = name,
                item = new Item[40],
                frame = frame,
                frameCounter = frameCounter
            };
            for (int j = 0; j < chest.item.Length; j++)
            {
                int netID = reader.ReadInt32();
                int stack = reader.ReadInt32();
                int prefix = reader.ReadByte();

                Item item = new();
                item.SetDefaults(netID);
                item.stack = stack;
                item.Prefix(prefix);

                chest.item[j] = item;
            }

            if (InZone(ref regions, x, y))
                continue;

            int chestIndex = Chest.FindChest(x, y);
            if (chestIndex == -1)
            {
                chestIndex = Chest.CreateChest(x, y);
                if (chestIndex != -1)
                {
                    Main.chest[chestIndex] = chest;
                    Main.chest[chestIndex].name = name;
                    for (int j = 0; j < chest.item.Length; j++)
                    {
                        Main.chest[chestIndex].item[j] = chest.item[j];
                    }
                }
            }
            else
            {
                Main.chest[chestIndex].name = name;
                for (int j = 0; j < chest.item.Length; j++)
                {
                    Main.chest[chestIndex].item[j] = chest.item[j];
                }
            }
        }

        foreach (PlayerEntity plr in EntityTrackers.Players)
        {
            for (int i = 0; i < Main.maxSectionsX; i++)
            {
                for (int j = 0; j < Main.maxSectionsY; j++)
                {
                    plr.Sections.UnmarkAsSent(i, j);
                }
            }
        }

        return true;
    }

    private struct RegionBounds
    {
        public RegionBounds(int x, int y, int x2, int y2)
        {
            X = x;
            Y = y;
            X2 = x2;
            Y2 = y2;
        }

        public int X, Y, X2, Y2;
    }
}