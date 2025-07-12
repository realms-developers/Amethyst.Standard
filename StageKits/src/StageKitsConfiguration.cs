using Amethyst.Network.Structures;
using Amethyst.Storages.Config;

namespace StageKits;

public sealed class StageKitsConfiguration
{
    public StageKitsConfiguration()
    {
        PrehardmodeItems = [];
        HardmodeItems = [];
    }

    public static Configuration<StageKitsConfiguration> Configuration { get; } = new Configuration<StageKitsConfiguration>("StageKits",
        new StageKitsConfiguration()
        {
            PrehardmodeLife = 120,
            PrehardmodeMana = 40,
            PrehardmodeItems =
            [
                new NetItem(Terraria.ID.ItemID.CopperShortsword, 1, 0),
                new NetItem(Terraria.ID.ItemID.CopperPickaxe, 1, 0),
                new NetItem(Terraria.ID.ItemID.CopperAxe, 1, 0),
                new NetItem(Terraria.ID.ItemID.CopperHammer, 1, 0),
                new NetItem(Terraria.ID.ItemID.Wood, 75, 0),
                new NetItem(Terraria.ID.ItemID.Torch, 25, 0),
                new NetItem(Terraria.ID.ItemID.Rope, 50, 0),
            ],

            HardmodeLife = 400,
            HardmodeMana = 100,
            HardmodeItems =
            [
                new NetItem(Terraria.ID.ItemID.MoltenPickaxe, 1, 0),
                new NetItem(Terraria.ID.ItemID.MoltenHamaxe, 1, 0),
                new NetItem(367, 1, 0),
                new NetItem(Terraria.ID.ItemID.Wood, 175, 0),
                new NetItem(Terraria.ID.ItemID.Torch, 25, 0),
                new NetItem(Terraria.ID.ItemID.Rope, 50, 0),
            ]
        });

    public static StageKitsConfiguration Instance => Configuration.Data;

    public int PrehardmodeLife;
    public int PrehardmodeMana;
    public NetItem[] PrehardmodeItems;

    public int HardmodeLife;
    public int HardmodeMana;
    public NetItem[] HardmodeItems;

    public int? PostMechanicLife;
    public int? PostMechanicMana;
    public NetItem[]? PostMechanicItems;

    public int? PostPlanteraLife;
    public int? PostPlanteraMana;
    public NetItem[]? PostPlanteraItems;

    public int? PostGolemLife;
    public int? PostGolemMana;
    public NetItem[]? PostGolemItems;

    public int? PostTowersLife;
    public int? PostTowersMana;
    public NetItem[]? PostTowersItems;
}