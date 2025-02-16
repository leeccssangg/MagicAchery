using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;

public class TalentTreeManager : Singleton<TalentTreeManager>
{
    private InGameDataManager InGameDataManagerCache { get; set; }
    private InGameDataManager InGameDataManager => InGameDataManagerCache ??= InGameDataManager.Instance;
    private TalentTreeData TalentTreeDataCache { get; set; } 
    private TalentTreeData TalentTreeData => TalentTreeDataCache ??= InGameDataManager.Instance.InGameData.TalentTreeData;
    private PlayerResourceData PlayerResourceDataCache { get; set; }
    private PlayerResourceData PlayerResourceData => PlayerResourceDataCache ??= InGameDataManager.Instance.InGameData.PlayerResourceData;
    [field: SerializeField] public ReactiveValue<int> FuryPoint {get; private set;}
    [field: SerializeField] public ReactiveValue<int> ArcanePoint {get; private set;}
    [field: SerializeField] public ReactiveValue<int> LuckPoint {get; private set;}
    private Dictionary<TalentStat.Type, TalentStat> TalentStatDictionary { get; set; } = new();
    private Dictionary<int, TalentTreeNodeData> TalentNodeDataDictionary { get; set; } = new();
    private Dictionary<int, TalentTreeNodeConfig> TalentTreeNodeConfig { get; set; } = new();
#if UNITY_EDITOR
    [ShowInInspector]
    private List<TalentStat> TalentStatList => new List<TalentStat>(TalentStatDictionary.Values);
#endif
    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        FuryPoint = TalentTreeData.FuryPoint;
        ArcanePoint = TalentTreeData.ArcanePoint;
        LuckPoint = TalentTreeData.FocusPoint;
        
        TalentTreeNodeConfig[] talentTreeNodeConfigs = TalentTreeGlobalConfig.Instance.GetAllTalentTreeNodeConfigs();
        foreach (TalentTreeNodeConfig talentTreeNodeConfig in talentTreeNodeConfigs)
        {
            TalentTreeNodeConfig[talentTreeNodeConfig.NodeId] = talentTreeNodeConfig;
        }

        foreach (TalentTreeNodeData talentNodeData in TalentTreeData.UnlockTalentNodeData)
        {
            TalentNodeDataDictionary[talentNodeData.NodeId] = talentNodeData;
            int nodeLevel = talentNodeData.NodeLevel.Value;
            TalentTreeNodeLevelConfig talentTreeNodeLevelConfig = TalentTreeNodeConfig[talentNodeData.NodeId].GetTalentTreeNodeLevelConfig(nodeLevel);
            AddTalentStat(talentTreeNodeLevelConfig.TalentStatGain);
        }
    }
    public bool IsTalentNodeUnlocked(int nodeId)
    {
        return TalentNodeDataDictionary.ContainsKey(nodeId);
    }

    public bool IsTalentNodeUnlockedAndMaxLevel(int nodeId)
    {
        if (!IsTalentNodeUnlocked(nodeId)) return false;
        return TalentNodeDataDictionary[nodeId].NodeLevel.Value >= TalentTreeNodeConfig[nodeId].MaxLevelUpgrade;
    }
    public TalentTreeNodeData GetTalentNodeData(int nodeId)
    {
        if (!TalentNodeDataDictionary.ContainsKey(nodeId))
        {
            TalentNodeDataDictionary[nodeId] = new TalentTreeNodeData(nodeId, new ReactiveValue<int>(0));
            TalentTreeData.UnlockTalentNodeData.Add(TalentNodeDataDictionary[nodeId]);
            InGameDataManager.SaveData();
        }
        return TalentNodeDataDictionary[nodeId];
    }
    public TalentStat GetTalentStat(TalentStat.Type talentStatType)
    {
        if (!TalentStatDictionary.ContainsKey(talentStatType))
        {
            TalentStatDictionary[talentStatType] = new TalentStat(talentStatType, BigNumber.ZERO);
        }
        return TalentStatDictionary[talentStatType];
    }
    private void AddTalentStat(TalentStat talentStat)
    {
        GetTalentStat(talentStat.StatType).Amount += talentStat.Amount;
    }
    private void RemoveTalentStat(TalentStat talentStat)
    {
        GetTalentStat(talentStat.StatType).Amount -= talentStat.Amount;
    }
    public void UpgradeTalentNode(int nodeId)
    {
        if (!IsTalentNodeUnlocked(nodeId)) return;
        TalentTreeNodeData talentNodeData = GetTalentNodeData(nodeId);
        if (talentNodeData.NodeLevel.Value >= TalentTreeNodeConfig[nodeId].MaxLevelUpgrade) return;
        TalentTreeNodeLevelConfig currentLevelConfig = TalentTreeNodeConfig[nodeId].GetTalentTreeNodeLevelConfig(talentNodeData.NodeLevel.Value);
        TalentTreeNodeLevelConfig nextLevelConfig = TalentTreeNodeConfig[nodeId].GetTalentTreeNodeLevelConfig(talentNodeData.NodeLevel.Value + 1);
        // GameResource gameResource = InGameDataManager.InGameData.PlayerResourceData.GetGameResource(nextLevelConfig.GameResourceRequire.ResourceType);
        // if (!PlayerResourceData.IsEnoughGameResource(gameResource)) return;
        // PlayerResourceData.ConsumeGameResource(gameResource);
        RemoveTalentStat(currentLevelConfig.TalentStatGain);
        talentNodeData.NodeLevel.Value++;
        AddTalentStat(nextLevelConfig.TalentStatGain);
        
        InGameDataManager.SaveData();
    }
}