using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;

[Serializable]

public class AchivementManager 
{
    [field: SerializeField] public AchivementData Data { get; private set; }
    [SerializeField] private AchivementConfigs m_Configs;
    [SerializeField] private AchivementSubject m_Subject = new();

    #region Save & Load Data
    public void LoadData(AchivementData data)
    {
        m_Configs = AchivementConfigs.Instance;
        Data = data;
        if(Data.EachAchivementsData.Count <= 0)
        {
            LoadAchivement();
            for (int i = 0; i < m_Subject.Achivements.Count; i++)
            {
                EachAchivementData eachAchivementData = new(m_Subject.Achivements[i]);
                ReactiveValue<EachAchivementData> reactiveValue = new();
                reactiveValue.Value = eachAchivementData;
                Data.EachAchivementsData.Add(reactiveValue);
            }
        }
        else
        {
            LoadAchivement(Data.EachAchivementsData);
        }
        //m_NextDay = Data.NextDay;
        //if (AllQuestManager.Instance.DailyQuestManager.IsNextDay())
        //{
        //    Data.QuestsData.Clear();
        //    //DailyQuestData.CurrentPoint.Value = 0;
        //    //DailyQuestData.CurrentStage.Value = 0;
        //    LoadQuests();
        //    for (int i = 0; i < m_Subject.DailyQuests.Count; i++)
        //    {
        //        QuestData questData = new(m_Subject.DailyQuests[i]);
        //        ReactiveValue<QuestData> reactiveValue = new ReactiveValue<QuestData>();
        //        reactiveValue.Value = questData;
        //        Data.QuestsData.Add(reactiveValue);
        //    }
        //    AllQuestManager.Instance.Notify(MissionTarget.LOGIN, "1");
        //}
        //else
        //{
        //    LoadQuests(Data.QuestsData);
        //}
        //pdateNextDay();
        AllQuestManager.Instance.SaveData();
    }
    public void ResetData()
    {
        LoadAchivement();
        for (int i = 0; i < Data.EachAchivementsData.Count; i++)
        {
            Data.EachAchivementsData[i].Value.Level.Value = m_Subject.Achivements[i].level;
            Data.EachAchivementsData[i].Value.Collect.Value = m_Subject.Achivements[i].collected;
        }
        AllQuestManager.Instance.SaveData();
    }
    public void LoadAchivement(List<ReactiveValue<EachAchivementData>> achivementsData = null)
    {
        m_Subject.LoadData(achivementsData);
    }
    public void SaveData()
    {
        
    }
    #endregion
    #region Manager Function
    public AchivementConfigs GetAchivementConfigs()
    {
        return m_Configs;
    }
    public AchivementConfig GetAchivementConfig(int id)
    {
        return m_Configs.GetAchivementConfig(id);
    }
    public int GetNumAchivementConfig()
    {
        return m_Configs.GetNumAchivementConfig();
    }
    public List<Achivement> GetListAchivement()
    {
        return m_Subject.GetListAchivement();
    }
    public int GetNumAchivement()
    {
        return m_Subject.GetNumAchivement();
    }
    #endregion
    #region Quest Stage
    //public void AddPoint()
    //{
    //    Debug.Log("add point");
    //    DailyQuestData.CurrentPoint.Value += 5;
    //    // AllQuestManager.Instance.Notify(MissionTarget.WATCH_ADS, "1");
    //    AllQuestManager.Instance.SaveData();
    //}
    //public int GetCurrentDailyQuestPoint()
    //{
    //    return DailyQuestData.CurrentPoint;
    //}
    //public int GetMaxDailyQuestPoint()
    //{
    //    return m_DailyQuestSubject.GetMaxPoint();
    //}
    //public float GetDailyProcess()
    //{
    //    float process = (float)GetCurrentDailyQuestPoint() / (float)m_DailyQuestSubject.GetMaxPoint();
    //    if (process >= 1) process = 1;
    //    return process;
    //}
    ////public int GetLastStageId()
    ////{
    ////    return m_DailyQuestConfigs.GetLastStageId();
    ////}
    ////public void ClaimDailyQuestStage()
    ////{
    ////    if (DailyQuestData.CurrentStage == GetLastStageId())
    ////    {
    ////        return;
    ////    }
    ////    DailyQuestData.CurrentStage.Value++;
    ////    //EventManager.TriggerEvent("ClaimDailyQuestStage");
    ////    AllQuestManager.Instance.SaveData();
    ////}
    //public int GetCurrentQuestStage()
    //{
    //    return DailyQuestData.CurrentStage;
    //}
    ////public bool IsGoodToClaimStage()
    ////{
    ////    int currentPoint = GetCurrentDailyQuestPoint();
    ////    if (DailyQuestData.CurrentStage == GetLastStageId())
    ////    {
    ////        return false;
    ////    }
    ////    if (currentPoint >= m_DailyQuestConfigs.questStages[DailyQuestData.CurrentStage].requiredPoint)
    ////    {
    ////        return true;
    ////    }
    ////    return false;
    ////}
    public bool IsGoodToClaimDailyQuest()
    {
        return m_Subject.IsGoodToAchivement();
    }
    #endregion
    #region Quest functions
    public void NotifyAchivement(MissionTarget questType, string info)
    {
        m_Subject.Notify(questType, info);
        for (int i = 0; i < m_Subject.Achivements.Count; i++)
        {
            if (m_Subject.Achivements[i].GetMissionTarget() == questType)
            {
                foreach (var q in Data.EachAchivementsData)
                {
                    if (q.ReactiveProperty.Value.Id == m_Subject.Achivements[i].id)
                    {
                        Debug.Log("quest notify");
                        //q.ReactiveProperty.Value = new(m_DailyQuestSubject.DailyQuests[i]);
                        q.ReactiveProperty.Value.Collect.Value = m_Subject.Achivements[i].collected;
                        break;
                    }
                }
                //QuestsData[i].ReactiveProperty.Value.Collect = new(DailyQuests[i].cl);
                //EventManager.TriggerEvent("QuestDataChange", QuestsData[i].Value);
                break;
            }
        }
    }
    public void NotifyAchivement(MissionTarget questType, int amount)
    {
        m_Subject.Notify(questType, amount.ToString());
    }
    public void ClaimAchivement(Achivement achivement)
    {
        m_Subject.ClaimAchivement(achivement);
        foreach (var q in Data.EachAchivementsData)
        {
            if (q.ReactiveProperty.Value.Id == achivement.id)
            {
                //q.ReactiveProperty.Value = new(quest);
                //q.ReactiveProperty.Value.IsClaimed.Value = achivement.icd;
                q.ReactiveProperty.Value.Level.Value = achivement.level;
                q.ReactiveProperty.Value.Collect.Value = achivement.collected;
                break;
            }
        }
        //DailyQuestData.CurrentPoint.Value += quest.GetPoint();
        AllQuestManager.Instance.SaveData();
    }
    public void ClaimAchivement(int id)
    {
        m_Subject.ClaimAchivement(id);
        foreach (var q in Data.EachAchivementsData)
        {
            if (q.ReactiveProperty.Value.Id == id)
            {
                //q.ReactiveProperty.Value = new(GetQuest(id));
                q.ReactiveProperty.Value.Level.Value = GetAchivement(id).level;
                break;
            }
        }
        //DailyQuestData.CurrentPoint.Value += GetQuest(id).GetPoint();
        AllQuestManager.Instance.SaveData();
    }
    public Achivement GetAchivement(int id)
    {
        return m_Subject.GetAchivement(id);
    }
    public void DoRandomAchivement()
    {
        int random = UnityEngine.Random.Range(0, m_Subject.GetNumAchivement());
        NotifyAchivement(m_Subject.GetAchivement(random).GetMissionTarget(), m_Subject.GetAchivement(random).targetAmount.ToString());
        //AllQuestManager.Instance.SaveData();
    }
    #endregion
    #region Editor
#if UNITY_EDITOR
    [Button]
    private void SaveDataEditor()
    {
        AllQuestManager.Instance.SaveData();
    }
#endif
    #endregion
}
