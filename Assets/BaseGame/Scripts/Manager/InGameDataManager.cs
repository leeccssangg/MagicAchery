using CodeStage.AntiCheat.Storage;
using MemoryPack;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UnityEngine;

public class InGameDataManager : Singleton<InGameDataManager>
{
    private const string KeyInGameData = "InGameData";
    [field: SerializeField] public InGameData InGameData { get; private set; } = new();

    protected override void Awake()
    {
        base.Awake();
#if !UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif
        LoadData();
    }

    [Button]
    public void SaveData()
    {
        ObscuredPrefs.Set(KeyInGameData, MemoryPackSerializer.Serialize(InGameData));
    }
    [Button]
    public void LoadData()
    {
        InGameData = MemoryPackSerializer.Deserialize<InGameData>(
            ObscuredPrefs.Get<byte[]>(KeyInGameData, MemoryPackSerializer.Serialize(new InGameData())));
    }
    [Button]
    public void ResetData()
    {
        InGameData = new InGameData();  
        SaveData();
    }
}

[System.Serializable]
[MemoryPackable]
public partial class InGameData
{

}