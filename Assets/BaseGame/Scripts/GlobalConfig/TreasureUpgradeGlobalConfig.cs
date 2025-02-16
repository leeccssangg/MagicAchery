using UnityEngine;
using Sirenix.Utilities;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TreasureUpgradeGlobalConfig", menuName = "GlobalConfigs/TreasureUpgradeGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class TreasureUpgradeGlobalConfig : GlobalConfig<TreasureUpgradeGlobalConfig>
{
    public List<TreasureUpgradeConfig> TreasureUpgradeConfigs = new();

    public TreasureUpgradeConfig GetTreasureUpgradeConfig(Rarity rarity)
    {
        for (int i = 0; i < TreasureUpgradeConfigs.Count; i++)
        {
            if (TreasureUpgradeConfigs[i].Rarity == rarity)
            {
                return TreasureUpgradeConfigs[i];
            }
        }
        return null;
    }
    public TreasureUpgradeLevelConfig GetTreasureUpgradeLevelConfig(Rarity rarity, int level)
    {
        TreasureUpgradeConfig treasureUpgradeConfig = GetTreasureUpgradeConfig(rarity);
        if (treasureUpgradeConfig != null)
        {
            for (int i = 0; i < treasureUpgradeConfig.LevelConfigs.Count; i++)
            {
                if (treasureUpgradeConfig.LevelConfigs[i].Level == level)
                {
                    return treasureUpgradeConfig.LevelConfigs[i];
                }
            }
        }
        return null;
    }
}
[System.Serializable]
public class TreasureUpgradeConfig
{
    public Rarity Rarity;
    public List<TreasureUpgradeLevelConfig> LevelConfigs;

    public int GetPieceCostUpgrade(int level)
    {
        for (int i = 0; i < LevelConfigs.Count; i++)
        {
            if (LevelConfigs[i].Level == level)
            {
                return LevelConfigs[i].PieceCost;
            }
        }
        return 0;
    }
    public GameResource GetGameResourceCostUpgrade(int level)
    {
        for (int i = 0; i < LevelConfigs.Count; i++)
        {
            if (LevelConfigs[i].Level == level)
            {
                return LevelConfigs[i].ResourceCost;
            }
        }
        return null;
    }

}
[System.Serializable]
public class TreasureUpgradeLevelConfig
{
    public int Level;
    public int PieceCost;
    public GameResource ResourceCost;
}