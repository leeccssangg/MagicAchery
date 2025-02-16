using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using TW.Utility.DesignPattern;
using UnityEngine;
using System;
using Unity.Jobs;
using Sirenix.OdinInspector;
using TW.Utility.Extension;

public class JobManager : Singleton<JobManager>
{
    private JobGlobalConfig JobGlobalConfig => JobGlobalConfig.Instance;
    [field: SerializeField] public int CurrentJobId { get; private set; }
    [field: SerializeField] public int CurrentJobLevel { get; private set; }

    #region Save & Load
    public void LoadData()
    {
        CurrentJobId = PlayerBattleData.Instance.JobId.Value;
        CurrentJobLevel = PlayerBattleData.Instance.JobLevel.Value;
    }
    public void SaveData()
    {
        PlayerBattleData.Instance.JobId.Value = CurrentJobId;
        PlayerBattleData.Instance.JobLevel.Value = CurrentJobLevel;
        InGameDataManager.Instance.SaveData();
    }
    #endregion
    #region Unity
    private void Start()
    {
        LoadData();
    }
    #endregion
    #region Manager
    public JobConfig GetJobConfig(int id)
    {
        return JobGlobalConfig.GetJobConfig(id);
    }
    public bool IsUpdradeAbleJob()
    {
        JobConfig config = GetJobConfig(CurrentJobId);
        return config.IsJobUpgradeAble(CurrentJobLevel);
    }
    public void UpgradeJob()
    {
        JobConfig config = GetJobConfig(CurrentJobId);
        if (config.IsJobMaxLevel(CurrentJobLevel))
        {
            UnlockNextJob();
        }
        else
        {
            CurrentJobLevel++;
        }
        SaveData();
    }
    public void UnlockNextJob()
    {
        CurrentJobId++;
        CurrentJobLevel = 1;
    }
    public GameResource GetJobReward()
    {
        JobConfig config = GetJobConfig(CurrentJobId);
        return config.GetJobReward(CurrentJobLevel);
    }
    public List<JobEffect> GetListJobEffect(int jobId)
    {
        JobConfig config = GetJobConfig(jobId);
        return config.JobEffect;
    }
    //private void DoJob(int jobId)
    //{
    //    Job job = GetJob(jobId);
    //    JobConfig config = GetJobConfig(jobId);
    //    if (job.JobProgress > 0)
    //    {
    //        m_TimeEndJob = DateTime.Now.AddSeconds(job.JobProgress);
    //        _awaiter?.Kill();
    //        _awaiter = new ARealtimeAwaiter(job.JobProgress, () =>
    //        {
    //            ClaimJobReward(jobId);
    //            DoJob(jobId);
    //        }, null)
    //            .OnUpdate(() => job.UpdateProgress((float)(m_TimeEndJob - DateTime.Now).TotalSeconds));
    //        SaveData();
    //    }
    //    else
    //    {
    //        m_TimeEndJob = DateTime.Now.AddSeconds(job.JobProgress);
    //        _awaiter?.Kill();
    //        _awaiter = new ARealtimeAwaiter(job.JobProgress, () =>
    //        {
    //            ClaimJobReward(jobId);
    //            DoJob(jobId);
    //        }, null)
    //            .OnUpdate(() => job.UpdateProgress((float)(m_TimeEndJob-DateTime.Now).TotalSeconds));
    //        SaveData();
    //    }
    //}
    private void ClaimJobReward()
    {
        GameResource reward = GetJobReward();
        PlayerResourceData.Instance.AddGameResource(reward);
    }
    #endregion
}
