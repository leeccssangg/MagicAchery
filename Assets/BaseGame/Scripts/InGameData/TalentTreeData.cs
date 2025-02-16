using System.Collections.Generic;
using MemoryPack;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class TalentTreeData
{
    private static TalentTreeData InstanceCache { get; set; }
    public static TalentTreeData Instance => InstanceCache ??= InGameDataManager.Instance.InGameData.TalentTreeData;
    [field: SerializeField] public ReactiveValue<int> FuryPoint {get; private set;}
    [field: SerializeField] public ReactiveValue<int> ArcanePoint {get; private set;}
    [field: SerializeField] public ReactiveValue<int> FocusPoint {get; private set;}
    [field: SerializeField] public List<TalentTreeNodeData> UnlockTalentNodeData {get; set;}
    
    [MemoryPackConstructor]
    public TalentTreeData()
    {
        UnlockTalentNodeData = new List<TalentTreeNodeData>();
    }
    public void AddTalentNodeData(TalentTreeNodeData talentTreeNodeData)
    {
        UnlockTalentNodeData.Add(talentTreeNodeData);
    }
}

[System.Serializable]
[MemoryPackable]
public partial class TalentTreeNodeData
{
    [field: SerializeField] public int NodeId {get; set;}
    [field: SerializeField] public ReactiveValue<int> NodeLevel {get; set;}
    
    [MemoryPackConstructor]
    public TalentTreeNodeData()
    {
        NodeLevel = new ReactiveValue<int>(0);
    }

    public TalentTreeNodeData(int nodeId, ReactiveValue<int> nodeLevel)
    {
        NodeId = nodeId;
        NodeLevel = nodeLevel;
    }
}

public partial class InGameData
{
    [MemoryPackOrder(5)] 
    [field: SerializeField, PropertyOrder(5)] public TalentTreeData TalentTreeData { get; set; } = new();
}