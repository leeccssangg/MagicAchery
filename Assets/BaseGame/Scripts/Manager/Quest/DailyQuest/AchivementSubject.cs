using Newtonsoft.Json;
//using R3;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using UnityEngine;

[System.Serializable]
public class AchivementSubject : Subject<MissionTarget>
{
    [field: SerializeField] public List<Achivement> Achivements { get; private set; } = new();

    #region Load & Save Data
    public void LoadData(List<ReactiveValue<EachAchivementData>> achivementDatas = null)
    {
        if (achivementDatas != null)
            LoadOldData(achivementDatas);
        else
            LoadNewData();
    }
    private void LoadNewData()
    {
        Achivements.Clear();
        Debug.Log("Load New Achivement Data");
        for (int i = 0; i < AllQuestManager.Instance.GetNumAchivementConfig(); i++)
        {
            AchivementConfig config = AllQuestManager.Instance.GetAchivementConfig(i);
            Achivement achivement = GenerateAchivement(config.missionTarget);
            achivement.Init(config.id);
            Achivements.Add(achivement);
            AddObserver(achivement);
            //ReactiveValue<QuestData> questData = new(new(DailyQuests[i]));
            //QuestsData.Add(questData);
        }
        //AllQuestManager.Instance.SaveData();
    }
    private void LoadOldData(List<ReactiveValue<EachAchivementData>> achivements)
    {
        //Debug.Log("Load Old Daily Quest Data");
        //List<DailyQuest> rawList = JsonConvert.DeserializeObject<List<DailyQuest>>(jsonData);
        //QuestsData = quests;
        for (int i = 0; i < achivements.Count; i++)
        {
            EachAchivementData data = achivements[i];
            Achivement achivement = new();
            achivement.Init(data.Id);
            //achivement.icd = data.IsClaimed;
            achivement.collected = data.Collect;
            achivement.level = data.Level;
            string rawString = JsonConvert.SerializeObject(achivement);
            Achivement newAchivement = DeserializeQuest(rawString, achivement.GetMissionTarget());
            newAchivement.InitConfig();
            Achivements.Add(newAchivement);
            AddObserver(newAchivement);
        }
    }
    public Achivement GenerateAchivement(MissionTarget missionTarget)
    {
        Achivement newAchivement = null;
        switch (missionTarget)
        {
            case MissionTarget.KILL_ENEMY:
                {
                    Achivement_KillEnemy newQuest = new();
                    newAchivement = newQuest;
                }
                break;
            case MissionTarget.DESTROY_OBSTACLE:
                {
                    Achivement_DestroyObstacle newQuest = new();
                    newAchivement = newQuest;
                }
                break;
            case MissionTarget.UPGRADE_ATK:
                {
                    Achivement_UpgradeATK newQuest = new();
                    newAchivement = newQuest;
                }
                break;
            case MissionTarget.UPGRADE_HP:
                {
                    Achivement_UpgradeHP newQuest = new();
                    newAchivement = newQuest;
                }
                break;
            case MissionTarget.UPGRADE_MANA:
                {
                    Achivement_UpgradeMana newQuest = new();
                    newAchivement = newQuest;
                }
                break;
            case MissionTarget.UPGRADE_MANA_REGEN:
                {
                    Achivement_UpgradeManaRegen newQuest = new();
                    newAchivement = newQuest;
                }
                break;
            case MissionTarget.LAND_HEADSHOT:
                {
                    Achivement_HeadShot newQuest = new();
                    newAchivement = newQuest;
                }
                break;
            case MissionTarget.PASS_LEVEL:
                {
                    Achivement_PassLevel newQuest = new();
                    newAchivement = newQuest;
                }
                break;
            case MissionTarget.WATCH_ADS:
                {
                    Achivement_WatchAds newQuest = new();
                    newAchivement = newQuest;
                }
                break;
            case MissionTarget.LOGIN:
                {
                    Achivement_Login newQuest = new();
                    newAchivement = newQuest;
                }
                break;
            case MissionTarget.UPGRADE_WEAPON:
                {
                    Achivement_UpgradeWeapon newQuest = new();
                    newAchivement = newQuest;
                }
                break;
            case MissionTarget.UPGRAGE_SKIN:
                {
                    Achivement_UpgradeSkin newQuest = new();
                    newAchivement = newQuest;
                }
                break;
        }
        return newAchivement;
    }
    public Achivement DeserializeQuest(string json, MissionTarget missionTarget)
    {
        Achivement achivement = null;
        switch (missionTarget)
        {
            case MissionTarget.KILL_ENEMY:
                {
                    achivement = JsonConvert.DeserializeObject<Achivement_KillEnemy>(json);
                }
                break;
            case MissionTarget.DESTROY_OBSTACLE:
                {
                    achivement = JsonConvert.DeserializeObject<Achivement_DestroyObstacle>(json);
                }
                break;
            case MissionTarget.UPGRADE_ATK:
                {
                    achivement = JsonConvert.DeserializeObject<Achivement_UpgradeATK>(json);
                }
                break;
            case MissionTarget.UPGRADE_HP:
                {
                    achivement = JsonConvert.DeserializeObject<Achivement_UpgradeHP>(json);
                }
                break;
            case MissionTarget.UPGRADE_MANA:
                {
                    achivement = JsonConvert.DeserializeObject<Achivement_UpgradeMana>(json);
                }
                break;
            case MissionTarget.UPGRADE_MANA_REGEN:
                {
                    achivement = JsonConvert.DeserializeObject<Achivement_UpgradeManaRegen>(json);
                }
                break;
            case MissionTarget.LAND_HEADSHOT:
                {
                    achivement = JsonConvert.DeserializeObject<Achivement_HeadShot>(json);
                }
                break;
            case MissionTarget.PASS_LEVEL:
                {
                    achivement = JsonConvert.DeserializeObject<Achivement_PassLevel>(json);
                }
                break;
            case MissionTarget.WATCH_ADS:
                {
                    achivement = JsonConvert.DeserializeObject<Achivement_WatchAds>(json);
                }
                break;
            case MissionTarget.LOGIN:
                {
                    achivement = JsonConvert.DeserializeObject<Achivement_Login>(json);
                }
                break;
            case MissionTarget.UPGRADE_WEAPON:
                {
                    achivement = JsonConvert.DeserializeObject<Achivement_UpgradeWeapon>(json);
                }
                break;
            case MissionTarget.UPGRAGE_SKIN:
                {
                    achivement = JsonConvert.DeserializeObject<Achivement_UpgradeSkin>(json);
                }
                break;
        }
        return achivement;
    }
    #endregion
    #region Achivement Function
    public override void Notify(MissionTarget id, string info)
    {
        base.Notify(id, info);
        AllQuestManager.Instance.SaveData();
    }
    public void ClaimAchivement(Achivement achivement)
    {
        achivement.OnClaim();
    }
    public void ClaimAchivement(int id)
    {
        for (int i = 0; i < Achivements.Count; i++)
        {
            if (Achivements[i].id == id)
            {
                Achivements[i].OnClaim();
            }
        }
    }
    public List<Achivement> GetListAchivement()
    {
        return new List<Achivement>(Achivements);
    }
    public Achivement GetAchivement(int id)
    {
        for (int i = 0; i < Achivements.Count; i++)
        {
            if (Achivements[i].id == id)
            {
                return Achivements[i];
            }
        }
        return null;
    }
    //public int GetMaxPoint()
    //{
    //    int maxPoint = 0;
    //    for (int i = 0; i < Achivements.Count; i++)
    //    {
    //        maxPoint += Achivements[i].GetPoint();
    //    }
    //    return maxPoint;
    //}
    //public int GetCurrentPoint()
    //{
    //    int currentPoint = 0;
    //    for (int i = 0; i < Achivements.Count; i++)
    //    {
    //        if (Achivements[i].IsClaimed())
    //            currentPoint += Achivements[i].GetPoint();
    //    }
    //    return currentPoint;
    //}
    //public float GetDailyProcess()
    //{
    //    float process = (float)GetCurrentPoint() / (float)GetMaxPoint();
    //    if (process >= 1) process = 1;
    //    return process;
    //}
    public int GetNumAchivement()
    {
        return Achivements.Count;
    }
    public bool IsGoodToAchivement()
    {
        for (int i = 0; i < Achivements.Count; i++)
        {
            if (!Achivements[i].IsMaxLevel() && Achivements[i].GetProgress() >= 1)
                return true;
        }
        return false;
    }
    public void DoRandomAchivement()
    {
        int questId = Random.Range(0, Achivements.Count);
        if (Achivements[questId].IsMaxLevel())
            return;
        Achivements[questId].OnNotify((MissionTarget)Achivements[questId].type, "1");
    }
    #endregion
}
