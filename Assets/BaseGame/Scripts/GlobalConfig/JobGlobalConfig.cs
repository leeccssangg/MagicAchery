using UnityEngine;
using Sirenix.Utilities;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "JobGlobalConfig", menuName = "GlobalConfigs/JobGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class JobGlobalConfig : GlobalConfig<JobGlobalConfig>
{
    public List<JobConfig> JobConfigs = new();

    public JobConfig GetJobConfig(int id)
    {
        if(id > JobConfigs[^1].Id)
        {
            return GetNewInfinityJobConfig(id);
        }
        else
        {
            for (int i = 0; i < JobConfigs.Count; i++)
            {
                if (JobConfigs[i].Id == id)
                {
                    return JobConfigs[i];
                }
            }
        }
        return null;
    }
    public bool IsMaxJob(int id)
    {
        JobConfig config = GetJobConfig(id);
        if (config.Id == JobConfigs[^1].Id)
            return true;
        return false;
    }
    private JobConfig GetNewInfinityJobConfig(int id)
    {
        int tmpId = id%JobConfigs[^1].Id;
        JobConfig config = GetJobConfig(tmpId);
        JobConfig newJobConfig = new();
        newJobConfig.Id = id;
        newJobConfig.Name = config.Name;
        newJobConfig.Reward = new(config.Reward.GameResourceData.ResourceType,Mathf.Pow(10,tmpId*0.36f));
        newJobConfig.MaxLevel = config.MaxLevel;
        newJobConfig.JobUpgradeRequirement = config.JobUpgradeRequirement;
        newJobConfig.JobEffect = config.JobEffect;
        return newJobConfig;
    }
}
[System.Serializable]
public class JobConfig
{
    public int Id;
    public string Name;
    public GameResource Reward;
    public int MaxLevel;
    public List<JobUpgradeRequirement> JobUpgradeRequirement;
    public List<JobEffect> JobEffect;

    public bool IsJobMaxLevel(int level)
    {
        return level == MaxLevel;
    }
    public bool IsJobUpgradeAble(int level)
    {
        for(int i = 0;i< JobUpgradeRequirement.Count; i++)
        {
            JobUpgradeRequirement jobUpgradeRequirement = JobUpgradeRequirement[i];
            if(!(PlayerStatData.Instance.GetGameResource(jobUpgradeRequirement.StatType).Level >=
                                (level * jobUpgradeRequirement.Multiplier + jobUpgradeRequirement.SideAmount)))
                return false;
        }
        return true;

    }
    public GameResource GetJobReward(int level)
    {
        return new(Reward.ResourceType,Reward.Amount*level);
    }
    public string GetJobRequireDescription(int level)
    {
        //return $"Require {JobUpgradeRequirement.StatType} Lv.{JobUpgradeRequirement.Multiplier*level + JobUpgradeRequirement.SideAmount}";
        return "";
    }
}
[System.Serializable]
public class JobUpgradeRequirement
{
    public GameStat.Type StatType;
    public int Multiplier;
    public int SideAmount;

    public int GetRequireLevel(int level)
    {
        return Multiplier * level + SideAmount;
    }
}
[System.Serializable]
public class JobEffect
{
    public GameStat.Type SkillType;
    public string Description;
}