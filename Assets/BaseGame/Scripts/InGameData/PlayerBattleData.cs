using MemoryPack;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class PlayerBattleData
{
    private static PlayerBattleData InstanceCache { get; set; }
    public static PlayerBattleData Instance => InstanceCache ??= InGameDataManager.Instance.InGameData.PlayerBattleData;
    
    //[field: SerializeField] public ReactiveValue<int> MapId { get; set; }
    [field: SerializeField] public ReactiveValue<GameStat.Type> CurrentTrainingStatType {get; private set;}
    [field: SerializeField] public ReactiveValue<int> JobId {get; private set;}
    [field: SerializeField] public ReactiveValue<int> JobLevel {get; private set;}
    [field: SerializeField] public ReactiveValue<int> TrainingAmount {get; private set;}
    [MemoryPackConstructor]
    public PlayerBattleData()
    {
        //MapId = new ReactiveValue<int>(1);
        CurrentTrainingStatType = new ReactiveValue<GameStat.Type>(GameStat.Type.Concentration);
        JobId = new ReactiveValue<int>(1);
        JobLevel = new ReactiveValue<int>(1);
        TrainingAmount = new ReactiveValue<int>(1);
    }
}
public partial class InGameData
{
    [MemoryPackOrder(3)] 
    [field: SerializeField, PropertyOrder(3)] public PlayerBattleData PlayerBattleData { get; set; } = new();
}