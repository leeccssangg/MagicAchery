using UnityEngine;
using System;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using TW.Reactive.CustomComponent;

public class AllQuestManager : Singleton<AllQuestManager>
{
    [field: SerializeField] public DailyQuestManager DailyQuestManager { get; private set; } = new();
    [field: SerializeField] public AchivementManager AchivementManager { get; private set; } = new();
    public ReactiveValue<TimeSpan> TimeToNextDay { get; private set; } = new();

    private void Start()
    {
        LoadData();
    }
    
    private void Update()
    {
        DateTime currentTime = DateTime.Now;
        if(DateTime.Compare(currentTime, DailyQuestManager.DailyQuestData.NextDay) >= 0)
        {
            DailyQuestManager.ResetData();
            //UpdateNextDay();
        }
        DateTime newDayTime = currentTime.AddDays(1);
        newDayTime = new DateTime(newDayTime.Year, newDayTime.Month, newDayTime.Day, 0, 0, 0);
        TimeToNextDay.Value = newDayTime.Subtract(currentTime);
    }
    public void LoadData()
    {
        DailyQuestManager.LoadData(InGameDataManager.Instance.InGameData.DailyQuestData);
        AchivementManager.LoadData(InGameDataManager.Instance.InGameData.AchivementData);
    }
    public void SaveData()
    {
        DailyQuestManager.SaveData();
        AchivementManager.SaveData();
        //InGameDataManager.Instance.InGameData.DailyQuestData = DailyQuestManager.DailyQuestData;
        InGameDataManager.Instance.SaveData();
        //InGameDataManager.Instance.SaveDailyQuestData(m_DailyQuestManager.SaveData());
    }
    public void Notify(MissionTarget id, string info)
    {
        DailyQuestManager.NotifyQuest(id, info);
        NotiDailyQuest();
        //RockieQuestManager.Ins.Notify(id, info);
        //UICMainMenu.Events.NotiDailyQuest?.Invoke();
    }
    #region Daily Quest
    public int GetNumDailyQuest()
    {
        return DailyQuestManager.GetNumDailyQuest();
    }
    public int GetNumDailyQuestConfig()
    {
        return DailyQuestManager.GetNumDailyQuestConfig();
    }
    public QuestConfig GetDailyQuestConfig(int id)
    {
        return DailyQuestManager.GetDailyQuestConfig(id);
    }
    public int GetCurrentDailyQuestPoint()
    {
        return DailyQuestManager.GetCurrentDailyQuestPoint();
    }
    public int GetDailyQuestMaxPoint()
    {
        return DailyQuestManager.GetMaxDailyQuestPoint();
    }
    public int GetCurrentDailyQuestStage()
    {
        return DailyQuestManager.GetCurrentQuestStage();
    }
    public DailyQuestConfigs GetDailyQuestConfigs()
    {
        return DailyQuestManager.GetDailyQuestConfigs();
    }
    public bool IsGoodToClaimDailyStageReward()
    {
        return DailyQuestManager.IsGoodToClaimStage();
    }
    public bool IsGoodToClaimDailyQuest()
    {
        return DailyQuestManager.IsGoodToClaimDailyQuest();
    }
    public int GetLastDailyQuestStageRewardId()
    {
        return DailyQuestManager.GetLastStageId();
    }
    public void ClaimDailyQuest(DailyQuest quest)
    {
        DailyQuestManager.ClaimQuest(quest);
        NotiDailyQuest();
        //UICMainMenu.Events.NotiDailyQuest?.Invoke();
    }
    public void ClaimDailyQuest(int id)
    {
        DailyQuestManager.ClaimQuest(id);
        NotiDailyQuest();
        //UICMainMenu.Events.NotiDailyQuest?.Invoke();
    }
    public DailyQuest GetDailyQuest(int id)
    {
        return DailyQuestManager.GetQuest(id);
    }
    public float GetDailyQuestProcess()
    {
        return DailyQuestManager.GetDailyProcess();
    }
    public void AddDailyQuestPoint()
    {
        DailyQuestManager.AddPoint();
        NotiDailyQuest();
        //UICMainMenu.Events.NotiDailyQuest?.Invoke();
    }
    public void ClaimDailyQuestStageReward()
    {
        DailyQuestManager.ClaimDailyQuestStage();
        NotiDailyQuest();
        //UICMainMenu.Events.NotiDailyQuest?.Invoke();
    }
    public bool IsShowNotiDailyQuest()
    {
        return IsGoodToClaimDailyQuest();
    }
    public void NotiDailyQuest()
    {
        //if (IsShowNotiDailyQuest())
        //    SheetMainMenu.Events.NotiDailyQuest?.Invoke(true);
        //else
        //    SheetMainMenu.Events.NotiDailyQuest?.Invoke(false);
    }
    #endregion
    #region Achivement
    public int GetNumAchivement()
    {
        return AchivementManager.GetNumAchivement();
    }
    public int GetNumAchivementConfig()
    {
        return AchivementManager.GetNumAchivementConfig();
    }
    public AchivementConfig GetAchivementConfig(int id)
    {
        return AchivementManager.GetAchivementConfig(id);
    }
    public AchivementConfigs GetAchivementConfigs()
    {
        return AchivementManager.GetAchivementConfigs();
    }
    public bool IsGoodToClaimAchivement()
    {
        return AchivementManager.IsGoodToClaimDailyQuest();
    }
    public void ClaimAchivement(Achivement achivement)
    {
        AchivementManager.ClaimAchivement(achivement);
        NotiAchivement();
        //UICMainMenu.Events.NotiDailyQuest?.Invoke();
    }
    public void ClaimAchivement(int id)
    {
        AchivementManager.ClaimAchivement(id);
        NotiAchivement();
        //UICMainMenu.Events.NotiDailyQuest?.Invoke();
    }
    public Achivement GetAchivement(int id)
    {
        return AchivementManager.GetAchivement(id);
    }
    public bool IsShowNotiAchivement()
    {
        return IsGoodToClaimAchivement();
    }
    public void NotiAchivement()
    {
        //if (IsShowNotiDailyQuest())
        //    SheetMainMenu.Events.NotiDailyQuest?.Invoke(true);
        //else
        //    SheetMainMenu.Events.NotiDailyQuest?.Invoke(false);
    }
    #endregion
    #region Editor
    [Button]
    public void DoRandomQuest()
    {
        DailyQuestManager.DoRandomQuest();
    }
    [Button]
    public void DoRandomAchivement()
    {
        AchivementManager.DoRandomAchivement();
    }
    #endregion
}
