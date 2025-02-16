using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;

[Serializable]
public class DailyQuestManager
{
    [field: SerializeField] public DailyQuestData DailyQuestData { get; private set; }
    [SerializeField] private DailyQuestConfigs m_DailyQuestConfigs;
    [SerializeField] private DailyQuestSubject m_DailyQuestSubject = new();

    private DateTime m_NextDay;

    #region Save & Load Data
    public void LoadData(DailyQuestData data)
    {
        m_DailyQuestConfigs = DailyQuestConfigs.Instance;
        DailyQuestData = data;
        m_NextDay = DailyQuestData.NextDay;
        if (IsNextDay())
        {
            DailyQuestData.QuestsData.Clear();
            DailyQuestData.CurrentPoint.Value = 0;
            DailyQuestData.CurrentStage.Value = 0;
            LoadQuests();
            for (int i = 0; i < m_DailyQuestSubject.DailyQuests.Count; i++)
            {
                QuestData questData = new(m_DailyQuestSubject.DailyQuests[i]);
                ReactiveValue<QuestData> reactiveValue = new ReactiveValue<QuestData>();
                reactiveValue.Value = questData;
                DailyQuestData.QuestsData.Add(reactiveValue);
            }
            AllQuestManager.Instance.Notify(MissionTarget.LOGIN, "1");
        }
        else
        {
            LoadQuests(DailyQuestData.QuestsData);
        }
        UpdateNextDay();
        AllQuestManager.Instance.SaveData();
    }
    public void ResetData()
    {
        LoadQuests();
        for (int i = 0; i < DailyQuestData.QuestsData.Count; i++)
        {
            DailyQuestData.QuestsData[i].Value.IsClaimed.Value = m_DailyQuestSubject.DailyQuests[i].icd;
            DailyQuestData.QuestsData[i].Value.Collect.Value = m_DailyQuestSubject.DailyQuests[i].cl;
        }
        DailyQuestData.CurrentPoint.Value = 0;
        DailyQuestData.CurrentStage.Value = 0;
        UpdateNextDay();
        AllQuestManager.Instance.SaveData();
    }
    public void LoadQuests(List<ReactiveValue<QuestData>> questData = null)
    {
        m_DailyQuestSubject.LoadData(questData);
    }
    public void SaveData()
    {
        DailyQuestData.NextDay = m_NextDay;
    }
    #endregion
    #region Manager Function
    private bool IsNextDay()
    {
        TimeSpan t = DateTime.Now - m_NextDay.Date;
        return t.TotalSeconds >= 0;
    }
    private void UpdateNextDay()
    {
        m_NextDay = TimeUtil.GetNextDate();
    }
    public DailyQuestConfigs GetDailyQuestConfigs()
    {
        return m_DailyQuestConfigs;
    } 
    public QuestConfig GetDailyQuestConfig(int id)
    {
        return m_DailyQuestConfigs.GetDailyQuestConfig(id);
    }
    public int GetNumDailyQuestConfig()
    {
        return m_DailyQuestConfigs.GetNumDailyQuestConfig();
    }
    public List<DailyQuest> GetListDailyQuests()
    {
        return m_DailyQuestSubject.GetListDailyQuests();
    }
    public int GetNumDailyQuest()
    {
        return m_DailyQuestSubject.GetNumDailyQuest();
    }
    #endregion
    #region Quest Stage
    public void AddPoint()
    {
        Debug.Log("add point");
        DailyQuestData.CurrentPoint.Value += 5;
        // AllQuestManager.Instance.Notify(MissionTarget.WATCH_ADS, "1");
        AllQuestManager.Instance.SaveData();
    }
    public int GetCurrentDailyQuestPoint()
    {
        return DailyQuestData.CurrentPoint;
    }
    public int GetMaxDailyQuestPoint()
    {
        return m_DailyQuestSubject.GetMaxPoint();
    }
    public float GetDailyProcess()
    {
        float process = (float)GetCurrentDailyQuestPoint() / (float)m_DailyQuestSubject.GetMaxPoint();
        if (process >= 1) process = 1;
        return process;
    }
    public int GetLastStageId()
    {
        return m_DailyQuestConfigs.GetLastStageId();
    }
    public void ClaimDailyQuestStage()
    {
        if (DailyQuestData.CurrentStage == GetLastStageId())
        {
            return;
        }
        DailyQuestData.CurrentStage.Value++;
        //EventManager.TriggerEvent("ClaimDailyQuestStage");
        AllQuestManager.Instance.SaveData();
    }
    public int GetCurrentQuestStage()
    {
        return DailyQuestData.CurrentStage;
    }
    public bool IsGoodToClaimStage()
    {
        int currentPoint = GetCurrentDailyQuestPoint();
        if (DailyQuestData.CurrentStage == GetLastStageId())
        {
            return false;
        }
        if (currentPoint >= m_DailyQuestConfigs.questStages[DailyQuestData.CurrentStage].requiredPoint)
        {
            return true;
        }
        return false;
    }
    public bool IsGoodToClaimDailyQuest()
    {
        return m_DailyQuestSubject.IsGoodToClaimQuest();
    }
    #endregion
    #region Quest functions
    public void NotifyQuest(MissionTarget questType, string info)
    {
        m_DailyQuestSubject.Notify(questType, info);
        for (int i = 0; i < m_DailyQuestSubject.DailyQuests.Count; i++)
        {
            if (m_DailyQuestSubject.DailyQuests[i].GetMissionTarget() == questType)
            {
                foreach (var q in DailyQuestData.QuestsData)
                {
                    if (q.ReactiveProperty.Value.Id == m_DailyQuestSubject.DailyQuests[i].id)
                    {
                        Debug.Log("quest notify");
                        //q.ReactiveProperty.Value = new(m_DailyQuestSubject.DailyQuests[i]);
                        q.ReactiveProperty.Value.Collect.Value = m_DailyQuestSubject.DailyQuests[i].cl;
                        break;
                    }
                }
                //QuestsData[i].ReactiveProperty.Value.Collect = new(DailyQuests[i].cl);
                //EventManager.TriggerEvent("QuestDataChange", QuestsData[i].Value);
                break;
            }
        }
    }
    public void NotifyQuest(MissionTarget questType, int amount)
    {
        m_DailyQuestSubject.Notify(questType, amount.ToString());
    }
    public void ClaimQuest(DailyQuest quest)
    {
        m_DailyQuestSubject.ClaimQuest(quest);
        foreach (var q in DailyQuestData.QuestsData)
        {
            if (q.ReactiveProperty.Value.Id == quest.id)
            {
                //q.ReactiveProperty.Value = new(quest);
                q.ReactiveProperty.Value.IsClaimed.Value = quest.icd;
                break;
            }
        }
        DailyQuestData.CurrentPoint.Value += quest.GetPoint();
        AllQuestManager.Instance.SaveData();
    }
    public void ClaimQuest(int id)
    {
        m_DailyQuestSubject.ClaimQuest(id);
        foreach (var q in DailyQuestData.QuestsData)
        {
            if (q.ReactiveProperty.Value.Id == id)
            {
                //q.ReactiveProperty.Value = new(GetQuest(id));
                q.ReactiveProperty.Value.IsClaimed.Value = GetQuest(id).icd;
                break;
            }
        }
        DailyQuestData.CurrentPoint.Value += GetQuest(id).GetPoint();
        AllQuestManager.Instance.SaveData();
    }
    public DailyQuest GetQuest(int id)
    {
        return m_DailyQuestSubject.GetQuest(id);
    }
    public void DoRandomQuest()
    {
        int random = UnityEngine.Random.Range(0, m_DailyQuestSubject.GetNumDailyQuest());
        NotifyQuest(m_DailyQuestSubject.GetQuest(random).GetMissionTarget(), m_DailyQuestSubject.GetQuest(random).tgm.ToString());
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
