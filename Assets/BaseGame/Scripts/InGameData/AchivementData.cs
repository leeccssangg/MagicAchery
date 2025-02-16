using MemoryPack;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using TW.Reactive.CustomComponent;

[System.Serializable]
[MemoryPackable]
public partial class AchivementData
{
    [field: SerializeField] public List<ReactiveValue<EachAchivementData>> EachAchivementsData { get; set; }

    [MemoryPackConstructor]
    public AchivementData()
    {
        EachAchivementsData = new();
    }
}
[System.Serializable]
[MemoryPackable]
public partial class EachAchivementData
{
    [field: SerializeField] public ReactiveValue<int> Id { get; set; }
    [field: SerializeField] public ReactiveValue<int> Collect { get; set; }
    [field: SerializeField] public ReactiveValue<int> Level { get; set; }

    [MemoryPackConstructor]
    public EachAchivementData()
    {
        Id = new(-1);
        Collect = new(0);
        Level = new(0);
    }
    public EachAchivementData(Achivement achivement)
    {
        Id = new(achivement.id);
        Collect = new(achivement.collected);
        Level = new(achivement.level);
    }
}
public partial class InGameData
{
    [MemoryPackOrder(8)]
    [field: SerializeField, PropertyOrder(8)] public AchivementData AchivementData { get; set; } = new();
}
