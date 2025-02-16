using System.Collections.Generic;
using UnityEngine;
using Sirenix.Utilities;
using TW.Reactive.CustomComponent;

[CreateAssetMenu(fileName = "AchivementConfigs", menuName = "GlobalConfigs/AchivementConfigs")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
[System.Serializable]
public class AchivementConfigs : GlobalConfig<AchivementConfigs>
{
    public List<AchivementConfig> achivementConfigs = new List<AchivementConfig>();
    //public List<QuestStage> questStages = new List<QuestStage>();

    public int GetNumAchivementConfig()
    {
        return achivementConfigs.Count;
    }
    public AchivementConfig GetAchivementConfig(int id)
    {
        for (int i = 0; i < achivementConfigs.Count; i++)
        {
            AchivementConfig config = achivementConfigs[i];
            if (config.id == id)
            {
                return config;
            }
        }
        return null;
    }
    public AchivementConfig GetAchivementConfig(MissionTarget missionTarget)
    {
        for (int i = 0; i < achivementConfigs.Count; i++)
        {
            AchivementConfig config = achivementConfigs[i];
            if (config.missionTarget == missionTarget)
            {
                return config;
            }
        }
        return null;
    }
    public List<AchivementConfig> GetAchivementConfigs()
    {
        return new List<AchivementConfig>(achivementConfigs);
    }
}
[System.Serializable]
public class AchivementConfig
{
    public int id;
    //public QuestCollectType type;
    public MissionTarget missionTarget;
    public int targetAmount;
    public List<GameResource> reward;
    public string description;
    public int offset;
    public int maxLevel;

    public string GetDescription()
    {
        return description;
    }
}

