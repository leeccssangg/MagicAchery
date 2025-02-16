using System.Collections.Generic;
using TW.Reactive.CustomComponent;

[System.Serializable]
public class Achivement : Achivement<MissionTarget>
{
    protected AchivementConfig config;
    public void Init(int id)
    {
        this.id = id;
        InitConfig();
        this.missionTarget = (int)config.missionTarget;
        //this.qt = (int)achivement.type;
        this.collected = 0;
        this.targetAmount = config.targetAmount;
        this.level = 0;
        this.offset = config.offset;
        this.maxLevel = config.maxLevel;
        //this.icd = 0;
        //this.pt = achivement.point;
    }
    public virtual void InitConfig()
    {
        config = AllQuestManager.Instance.GetAchivementConfig(id);
    }
    public override void OnNotify(MissionTarget id, string info)
    {
        if (id == config.missionTarget)
        {
            base.OnNotify(id, info);
        }
    }
    public virtual MissionTarget GetMissionTarget()
    {
        return (MissionTarget)this.missionTarget;
    }
    public void SetMissionTarget(MissionTarget missionTarget)
    {
        this.missionTarget = (int)missionTarget;
    }
    public virtual void SetTargetAmount(int amount)
    {
        this.targetAmount = amount;
    }
    public void SetQuestConfig(AchivementConfig questConfig)
    {
        this.config = questConfig;
    }
    public AchivementConfig GetQuestConfig()
    {
        return config;
    }
    public override string GetDescription()
    {
        return config.GetDescription();
    }
    public List<GameResource> GetReward()
    {
        return config.reward;
    }
}
