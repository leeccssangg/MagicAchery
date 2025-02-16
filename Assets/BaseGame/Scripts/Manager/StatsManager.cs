using R3;
using TW.ACacheEverything;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;
using MemoryPack;
using Sirenix.OdinInspector;

public class StatsManager : Singleton<StatsManager>
{
    [field: SerializeField] public StatsData StatsData { get; private set; }
    private PlayerStatData PlayerStatDataCache { get; set; }
    private PlayerStatData PlayerStatData => PlayerStatDataCache ??= PlayerStatData.Instance;
    private StatsUnlockGlobalConfig StatsUnlockGlobalConfigCache => StatsUnlockGlobalConfig.Instance;
    [field: SerializeField] public ReactiveValue<GameStat.Type> CurrentTrainingStatType { get; private set; }

    #region Unity Functions 
    private void Start()
    {
        LoadData();
        CurrentTrainingStatType = PlayerBattleData.Instance.CurrentTrainingStatType;
        //CurrentTrainingStatType.ReactiveProperty.Subscribe(OnTrainingStatTypeChange).AddTo(this);
    }
    #endregion
    #region Save & Load
    private void LoadData()
    {
        StatsData = InGameDataManager.Instance.InGameData.StatsData;
    }
    private void SaveData()
    {
        InGameDataManager.Instance.InGameData.StatsData = StatsData;
        InGameDataManager.Instance.SaveData();
    }
    #endregion
    #region Manager functions
    public void UnlockStat(int id)
    {
        StatsData.UnlockedId.Value = id;
        SaveData();
    }
    public StatUnlock GetStatsUnlock(int id)
    {
        return StatsUnlockGlobalConfigCache.GetStatUnlock(id);
    }
    public bool IsUnlockedAllStats()
    {
        return StatsData.UnlockedId >= StatsUnlockGlobalConfigCache.statUnlocks[^1].id;
    }
    public GameResource GetCostUnlockNextStat()
    {
        if(IsUnlockedAllStats())
        {
            return new GameResource();
        }
        return GetStatsUnlock(StatsData.UnlockedId + 1).cost;
    }
    public bool IsUnlockAbleNextStat()
    {
        if(IsUnlockedAllStats())
        {
            return false;
        }
        return PlayerResourceData.Instance.IsEnoughGameResource(GetCostUnlockNextStat());
    }
    public bool IsStatsUnlocked(GameStat.Type type)
    {
        return StatsUnlockGlobalConfigCache.IsStatUnlocked(type,StatsData.UnlockedId);
    }
    public bool IsStatsUnlockNext(GameStat.Type type)
    {
        return StatsUnlockGlobalConfigCache.IsStatUnlockNext(type,StatsData.UnlockedId);
    }
    #endregion
}
[System.Serializable]
[MemoryPackable]
public partial class StatsData
{
    public ReactiveValue<int> UnlockedId;

    [MemoryPackConstructor]
    public StatsData()
    {
        UnlockedId = new(0);
    }
}
public partial class InGameData
{
    [MemoryPackOrder(4)]
    [field: SerializeField, PropertyOrder(4)] public StatsData StatsData { get; set; } = new();
}
