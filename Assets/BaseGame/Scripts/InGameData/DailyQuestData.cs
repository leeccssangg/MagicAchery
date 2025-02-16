using MemoryPack;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using TW.Reactive.CustomComponent;

[System.Serializable]
[MemoryPackable]
public partial class DailyQuestData
{
    [field: SerializeField] public DateTime NextDay { get; set; }
    [field: SerializeField] public List<ReactiveValue<QuestData>> QuestsData { get; set; }
    [field: SerializeField] public ReactiveValue<int> CurrentStage { get; set; }
    [field: SerializeField] public ReactiveValue<int> CurrentPoint { get; set; }

    [MemoryPackConstructor]
    public DailyQuestData()
    {
        NextDay = DateTime.Now;
        QuestsData = new();
        CurrentStage = new(0);
        CurrentPoint = new(0);
    }
}
[System.Serializable]
[MemoryPackable]
public partial class QuestData
{
    [field: SerializeField] public ReactiveValue<int> Id { get; set; }
    [field: SerializeField] public ReactiveValue<int> Collect { get; set; }
    [field: SerializeField] public ReactiveValue<int> IsClaimed { get; set; }

    [MemoryPackConstructor]
    public QuestData()
    {
        Id = new(-1);
        Collect = new(0);
        IsClaimed = new(0);
    }
    public QuestData (DailyQuest dailyQuest)
    {
        Id = new(dailyQuest.id);
        Collect = new(dailyQuest.cl);
        IsClaimed = new(dailyQuest.icd);
    }
}
public partial class InGameData
{
    [MemoryPackOrder(3)]
    [field: SerializeField, PropertyOrder(3)] public DailyQuestData DailyQuestData { get; set; } = new();
}
