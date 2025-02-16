using MemoryPack;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class SettingData
{
    public static SettingData Instance => InGameDataManager.Instance.InGameData.SettingData;
    [field: SerializeField] public ReactiveValue<bool> SoundActive { get; set; } = new(true);
    [field: SerializeField] public ReactiveValue<bool> MusicActive { get; set; } = new(true);
    [field: SerializeField] public ReactiveValue<bool> VibrateActive { get; set; } = new(true);
}

public partial class InGameData
{
    [MemoryPackOrder(3)] 
    [field: SerializeField, PropertyOrder(3)] public SettingData SettingData { get; set; } = new();
}