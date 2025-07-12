using Amethyst.Network.Structures;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Factories;
using Amethyst.Systems.Characters.Enums;
using Amethyst.Systems.Characters.Utilities;
using Terraria;

namespace StageKits;

public sealed class StageModelFactory : IDefaultModelFactory
{
    public ICharacterModel CreateModel(PlayerEntity player)
    {
        EmptyCharacterModel model = new();
        model.Name = player.Name;

        CopyPlayerInfo(player, ref model);
        CopyFromConfig(ref model);

        return model;
    }

    private void CopyPlayerInfo(PlayerEntity player, ref EmptyCharacterModel model)
    {
        model.Hair = player.TempPlayerInfo.HairID;
        model.HairDye = player.TempPlayerInfo.HairDyeID;
        model.SkinVariant = player.TempPlayerInfo.SkinVariant;

        model.Colors[(byte)PlayerColorType.SkinColor] = player.TempPlayerInfo.SkinColor;
        model.Colors[(byte)PlayerColorType.HairColor] = player.TempPlayerInfo.HairColor;
        model.Colors[(byte)PlayerColorType.ShirtColor] = player.TempPlayerInfo.ShirtColor;
        model.Colors[(byte)PlayerColorType.UnderShirtColor] = player.TempPlayerInfo.UnderShirtColor;
        model.Colors[(byte)PlayerColorType.PantsColor] = player.TempPlayerInfo.PantsColor;
        model.Colors[(byte)PlayerColorType.ShoesColor] = player.TempPlayerInfo.ShoeColor;
        model.Colors[(byte)PlayerColorType.EyesColor] = player.TempPlayerInfo.EyeColor;
    }

    private void CopyFromConfig(ref EmptyCharacterModel model)
    {
        (int life, int mana, NetItem[] items) = GetStageInventory();
        
        for (int i = 0; i < items.Length; i++)
        {
            model.Slots[i] = items[i];
        }

        model.MaxLife = life;
        model.MaxMana = mana;
    }

    private (int Life, int Mana, NetItem[]) GetStageInventory()
    {
        int maxlife = NPC.downedTowers && StageKitsConfiguration.Instance.PostTowersLife != null ? StageKitsConfiguration.Instance.PostTowersLife.Value :
                        NPC.downedGolemBoss && StageKitsConfiguration.Instance.PostGolemLife != null ? StageKitsConfiguration.Instance.PostGolemLife.Value :
                        NPC.downedPlantBoss && StageKitsConfiguration.Instance.PostPlanteraLife != null ? StageKitsConfiguration.Instance.PostPlanteraLife.Value :
                        NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && StageKitsConfiguration.Instance.PostMechanicLife != null ? StageKitsConfiguration.Instance.PostMechanicLife.Value :
                        Main.hardMode ? StageKitsConfiguration.Instance.HardmodeLife : StageKitsConfiguration.Instance.PrehardmodeLife;

        int maxmana = NPC.downedTowers && StageKitsConfiguration.Instance.PostTowersMana != null ? StageKitsConfiguration.Instance.PostTowersMana.Value :
                        NPC.downedGolemBoss && StageKitsConfiguration.Instance.PostGolemMana != null ? StageKitsConfiguration.Instance.PostGolemMana.Value :
                        NPC.downedPlantBoss && StageKitsConfiguration.Instance.PostPlanteraMana != null ? StageKitsConfiguration.Instance.PostPlanteraMana.Value :
                        NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && StageKitsConfiguration.Instance.PostMechanicMana != null ? StageKitsConfiguration.Instance.PostMechanicMana.Value :
                        Main.hardMode ? StageKitsConfiguration.Instance.HardmodeMana : StageKitsConfiguration.Instance.PrehardmodeMana;

        NetItem[] items = NPC.downedTowers && StageKitsConfiguration.Instance.PostTowersItems != null ? StageKitsConfiguration.Instance.PostTowersItems :
                        NPC.downedGolemBoss && StageKitsConfiguration.Instance.PostGolemItems != null ? StageKitsConfiguration.Instance.PostGolemItems :
                        NPC.downedPlantBoss && StageKitsConfiguration.Instance.PostPlanteraItems != null ? StageKitsConfiguration.Instance.PostPlanteraItems :
                        NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && StageKitsConfiguration.Instance.PostMechanicItems != null ? StageKitsConfiguration.Instance.PostMechanicItems :
                        Main.hardMode ? StageKitsConfiguration.Instance.HardmodeItems : StageKitsConfiguration.Instance.PrehardmodeItems;

        return (maxlife, maxmana, items);
    }
}
