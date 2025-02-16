using System;
using TMPro;
using TW.Utility.CustomComponent;
using UnityEngine;

public class UIStatsRequire : ACachedMonoBehaviour
{
    [field: SerializeField] public GameStat.Type StatType { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtLevelRequire { get; private set; }
    private IDisposable disposable;

    //private void Awake()
    //{
    //    //PlayerBattleData.Instance.JobId.ReactiveProperty
    //    //    .CombineLatest(PlayerBattleData.Instance.JobLevel.ReactiveProperty,(i,l) => (i,l))
    //    //    .Subscribe(Setup)
    //    //    .AddTo(this);
    //}

    public void Setup(int id)
    {
        //StatType = type;
        if(id == -1)
        {
            TxtLevelRequire.gameObject.SetActive(false);
        }
        else
        {
            TxtLevelRequire.gameObject.SetActive(true);
            JobConfig config = JobGlobalConfig.Instance.GetJobConfig(PlayerBattleData.Instance.JobId);
            StatType = config.JobUpgradeRequirement[id].StatType;
            TxtLevelRequire.SetText($"- {StatType.ToString()}: Lv.{config.JobUpgradeRequirement[id].GetRequireLevel(PlayerBattleData.Instance.JobLevel)}");
        }
    }
}
