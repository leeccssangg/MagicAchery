using System.Collections.Generic;
using MemoryPack;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class PlayerStatData
{
    private static PlayerStatData InstanceCache { get; set; }
    public static PlayerStatData Instance => InstanceCache ??= InGameDataManager.Instance.InGameData.PlayerStatData;
    [field: SerializeField] public List<GameStat> GameStatList { get; set; }
    public GameStat this[GameStat.Type statType] => GetGameResource(statType);
    [MemoryPackConstructor]
    public PlayerStatData()
    {
        GameStatList = new List<GameStat>()
        {
            new GameStat(GameStat.Type.Concentration, 0, 0),
            new GameStat(GameStat.Type.Strength, 0, 0),
            new GameStat(GameStat.Type.Dexterity, 0, 0),
            new GameStat(GameStat.Type.Accuracy, 0, 0),
            new GameStat(GameStat.Type.Vitality, 0, 0),
            new GameStat(GameStat.Type.Intelligence, 0, 0),
            new GameStat(GameStat.Type.Wisdom, 0, 0),
            new GameStat(GameStat.Type.Luck, 0, 0),
            new GameStat(GameStat.Type.Charisma, 0, 0),
        };
    }

    public GameStat GetGameResource(GameStat.Type statType)
    {
        GameStat stat = null;
        for (int i = 0; i < GameStatList.Count; i++)
        {
            if (GameStatList[i].StatType == statType)
            {
                stat = GameStatList[i];
                break;
            }
        }
        if (stat == null)
        {
            stat = new GameStat(statType, 1, 0);
            GameStatList.Add(stat);
        }
        return stat;
    }
}

public partial class InGameData
{
    [MemoryPackOrder(1)] 
    [field: SerializeField, PropertyOrder(1)] public PlayerStatData PlayerStatData { get; set; } = new();
}