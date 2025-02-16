using Newtonsoft.Json;
//using R3;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using UnityEngine;

[System.Serializable]
public class DailyQuestSubject : Subject<MissionTarget>
{
    [field : SerializeField] public List<DailyQuest> DailyQuests { get; private set; } = new();

    #region Load & Save Data
    public void LoadData(List<ReactiveValue<QuestData>> quests = null)
    {
        if (quests != null)
            LoadOldData(quests);
        else
            LoadNewData();
    }
    private void LoadNewData()
    {
        DailyQuests.Clear();
        Debug.Log("Load New Daily Quest Data");
        for (int i = 0; i < AllQuestManager.Instance.GetNumDailyQuestConfig(); i++)
        {
            QuestConfig questConfig = AllQuestManager.Instance.GetDailyQuestConfig(i);
            DailyQuest dailyQuest = GenerateDailyQuest(questConfig.missionTarget);
            dailyQuest.Init(questConfig.id);
            DailyQuests.Add(dailyQuest);
            AddObserver(dailyQuest);
            //ReactiveValue<QuestData> questData = new(new(DailyQuests[i]));
            //QuestsData.Add(questData);
        }
        //AllQuestManager.Instance.SaveData();
    }
    private void LoadOldData(List<ReactiveValue<QuestData>> quests)
    {
        //Debug.Log("Load Old Daily Quest Data");
        //List<DailyQuest> rawList = JsonConvert.DeserializeObject<List<DailyQuest>>(jsonData);
        //QuestsData = quests;
        for (int i = 0; i < quests.Count; i++)
        {
            QuestData dailyQuestData = quests[i];
            DailyQuest dailyQuest = new();
            dailyQuest.Init(dailyQuestData.Id);
            dailyQuest.icd = dailyQuestData.IsClaimed;
            dailyQuest.cl = dailyQuestData.Collect;
            string rawString = JsonConvert.SerializeObject(dailyQuest);
            DailyQuest newQuest = DeserializeQuest(rawString, dailyQuest.GetMissionTarget());
            newQuest.InitQuestConfig();
            DailyQuests.Add(newQuest);
            AddObserver(newQuest);
        }
    }
    public DailyQuest GenerateDailyQuest(MissionTarget missionTarget)
    {
        DailyQuest newDailyQuest = null;
        switch (missionTarget)
        {
            case MissionTarget.KILL_ENEMY:
                {
                    DailyQuest_KillEnemy newQuest = new();
                    newDailyQuest = newQuest;
                }
                break;
            case MissionTarget.DESTROY_OBSTACLE:
                {
                    DailyQuest_DestroyObstacle newQuest = new();
                    newDailyQuest = newQuest;
                }
                break;
            case MissionTarget.UPGRADE_ATK:
                {
                    DailyQuest_UpgradeATK newQuest = new();
                    newDailyQuest = newQuest;
                }
                break;
            case MissionTarget.UPGRADE_HP:
                {
                    DailyQuest_UpgradeHP newQuest = new();
                    newDailyQuest = newQuest;
                }
                break;
            case MissionTarget.UPGRADE_MANA:
                {
                    DailyQuest_UpgradeMana newQuest = new();
                    newDailyQuest = newQuest;
                }
                break;
            case MissionTarget.UPGRADE_MANA_REGEN:
                {
                    DailyQuest_UpgradeManaRegen newQuest = new();
                    newDailyQuest = newQuest;
                }
                break;
            case MissionTarget.LAND_HEADSHOT:
                {
                    DailyQuest_HeadShot newQuest = new();
                    newDailyQuest = newQuest;
                }
                break;
            case MissionTarget.PASS_LEVEL:
                {
                    DailyQuest_PassLevel newQuest = new();
                    newDailyQuest = newQuest;
                }
                break;
            case MissionTarget.WATCH_ADS:
                {
                    DailyQuest_WatchAds newQuest = new();
                    newDailyQuest = newQuest;
                }
                break;
            case MissionTarget.LOGIN:
                {
                    DailyQuest_Login newQuest = new();
                    newDailyQuest = newQuest;
                }
                break;
            case MissionTarget.UPGRADE_WEAPON:
                {
                    DailyQuest_UpgradeWeapon newQuest = new();
                    newDailyQuest = newQuest;
                }
                break;
            case MissionTarget.UPGRAGE_SKIN:
                {
                    DailyQuest_UpgradeSkin newQuest = new();
                    newDailyQuest = newQuest;
                }
                break;
        }
        return newDailyQuest;
    }
    public DailyQuest DeserializeQuest(string json, MissionTarget missionTarget)
    {
        DailyQuest dailyQuest = null;
        switch (missionTarget)
        {
            case MissionTarget.KILL_ENEMY:
                {
                    dailyQuest = JsonConvert.DeserializeObject<DailyQuest_KillEnemy>(json);
                }
                break;
            case MissionTarget.DESTROY_OBSTACLE:
                {
                    dailyQuest = JsonConvert.DeserializeObject<DailyQuest_DestroyObstacle>(json);
                }
                break;
            case MissionTarget.UPGRADE_ATK:
                {
                    dailyQuest = JsonConvert.DeserializeObject<DailyQuest_UpgradeATK>(json);
                }
                break;
            case MissionTarget.UPGRADE_HP:
                {
                    dailyQuest = JsonConvert.DeserializeObject<DailyQuest_UpgradeHP>(json);
                }
                break;
            case MissionTarget.UPGRADE_MANA:
                {
                    dailyQuest = JsonConvert.DeserializeObject<DailyQuest_UpgradeMana>(json);
                }
                break;
            case MissionTarget.UPGRADE_MANA_REGEN:
                {
                    dailyQuest = JsonConvert.DeserializeObject<DailyQuest_UpgradeManaRegen>(json);
                }
                break;
            case MissionTarget.LAND_HEADSHOT:
                {
                    dailyQuest = JsonConvert.DeserializeObject<DailyQuest_HeadShot>(json);
                }
                break;
            case MissionTarget.PASS_LEVEL:
                {
                    dailyQuest = JsonConvert.DeserializeObject<DailyQuest_PassLevel>(json);
                }
                break;
            case MissionTarget.WATCH_ADS:
                {
                    dailyQuest = JsonConvert.DeserializeObject<DailyQuest_WatchAds>(json);
                }
                break;
            case MissionTarget.LOGIN:
                {
                    dailyQuest = JsonConvert.DeserializeObject<DailyQuest_Login>(json);
                }
                break;
            case MissionTarget.UPGRADE_WEAPON:
                {
                    dailyQuest = JsonConvert.DeserializeObject<DailyQuest_UpgradeWeapon>(json);
                }
                break;
            case MissionTarget.UPGRAGE_SKIN:
                {
                    dailyQuest = JsonConvert.DeserializeObject<DailyQuest_UpgradeSkin>(json);
                }
                break;
        }
        return dailyQuest;
    }
    #endregion
    #region Daily Quest Function
    public override void Notify(MissionTarget id, string info)
    {
        base.Notify(id, info);
        AllQuestManager.Instance.SaveData();
    }
    public void ClaimQuest(DailyQuest dailyQuest)
    {
        dailyQuest.OnClaim();
    }
    public void ClaimQuest(int questId)
    {
        for (int i = 0; i < DailyQuests.Count; i++)
        {
            if (DailyQuests[i].id == questId)
            {
                DailyQuests[i].OnClaim();
            }
        }
    }
    public List<DailyQuest> GetListDailyQuests()
    {
        return new List<DailyQuest>(DailyQuests);
    }
    public DailyQuest GetQuest(int questId)
    {
        for (int i = 0; i < DailyQuests.Count; i++)
        {
            if (DailyQuests[i].id == questId)
            {
                return DailyQuests[i];
            }
        }
        return null;
    }
    public int GetMaxPoint()
    {
        int maxPoint = 0;
        for (int i = 0; i < DailyQuests.Count; i++)
        {
            maxPoint += DailyQuests[i].GetPoint();
        }
        return maxPoint;
    }
    public int GetCurrentPoint()
    {
        int currentPoint = 0;
        for (int i = 0; i < DailyQuests.Count; i++)
        {
            if (DailyQuests[i].IsClaimed())
                currentPoint += DailyQuests[i].GetPoint();
        }
        return currentPoint;
    }
    public float GetDailyProcess()
    {
        float process = (float)GetCurrentPoint() / (float)GetMaxPoint();
        if (process >= 1) process = 1;
        return process;
    }
    public int GetNumDailyQuest()
    {
        return DailyQuests.Count;
    }
    public bool IsGoodToClaimQuest()
    {
        for(int i = 0;i<DailyQuests.Count;i++)
        {
            if (!DailyQuests[i].IsClaimed() && DailyQuests[i].GetProgress() >= 1)
                return true;
        }
        return false;
    } 
    public void DoRandomQuest()
    {
        int questId = Random.Range(0, DailyQuests.Count);
        if (DailyQuests[questId].IsClaimed())
            return;
        DailyQuests[questId].OnNotify((MissionTarget)DailyQuests[questId].mt,"1");
    }
    #endregion

}
