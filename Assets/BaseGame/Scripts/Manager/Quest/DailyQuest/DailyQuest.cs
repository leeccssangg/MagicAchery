using System.Collections.Generic;

[System.Serializable]
public class DailyQuest : Quest<MissionTarget>
{
    protected QuestConfig questConfig;
    public void Init(int id)
    {
        this.id = id;
        InitQuestConfig();
        this.mt = (int)questConfig.missionTarget;
        this.qt = (int)questConfig.type;
        this.cl = 0;
        this.tgm = questConfig.targetAmount;
        this.icd = 0;
        this.pt = questConfig.point;
    }
    public virtual void InitQuestConfig()
    {
        questConfig = AllQuestManager.Instance.GetDailyQuestConfig(id);
    }
    public override void OnNotify(MissionTarget id, string info)
    {
        if (id == questConfig.missionTarget)
        {
            base.OnNotify(id, info);
        }
    }
    public virtual MissionTarget GetMissionTarget()
    {
        return (MissionTarget)this.mt;
    }
    public void SetMissionTarget(MissionTarget missionTarget)
    {
        this.mt = (int)missionTarget;
    }
    public virtual void SetTargetAmount(int amount)
    {
        this.tgm = amount;
    }
    public void SetQuestConfig(QuestConfig questConfig)
    {
        this.questConfig = questConfig;
    }
    public QuestConfig GetQuestConfig()
    {
        return questConfig;
    }
    public override string GetDescription()
    {
        return questConfig.GetDescription();
    }
    public List<GameResource> GetReward()
    {
        return questConfig.reward;
    }
}
