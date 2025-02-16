using MemoryPack;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class TreasureData 
{
    [field: SerializeField] public TreasureUpgradeData TreasureUpgradeData { get; set; } = new();
    [field: SerializeField] public TreasureSlotData TreasureSlotData { get; set; } = new();
}
[System.Serializable]
[MemoryPackable]
public partial class TreasureSlotData
{
    [field: SerializeField] public Dictionary<int, ReactiveValue<int>> SlotData { get; private set; } = new();
}
public partial class InGameData
{
    [MemoryPackOrder(6)]
    [field: SerializeField, PropertyOrder(6)] public TreasureData TreasureData { get; set; } = new();
}
