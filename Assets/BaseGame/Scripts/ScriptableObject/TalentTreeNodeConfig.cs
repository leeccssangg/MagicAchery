using System;
using Sirenix.OdinInspector;
using TW.Utility.CustomScaleMethod;
using TW.Utility.CustomType;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TalentTreeNodeConfig", menuName = "ScriptableObject/TalentTreeNodeConfig")]
public class TalentTreeNodeConfig : ScriptableObject
{
    private const string GroupName1 = "TalentTreeNodeConfig";
    private const string GroupName2 = "TalentTreeNodeConfig/Stat";

    [field: HorizontalGroup(GroupName1, width: 200)]
    [field: PreviewField(ObjectFieldAlignment.Center, Height = 180), HideLabel]
    [field: SerializeField]
    public Sprite NodeIcon { get; private set; }

    [field: SerializeReference, VerticalGroup(GroupName2)] public int NodeId { get; private set; }

    [field: SerializeReference, VerticalGroup(GroupName2)] public string NodeName { get; private set; }
    [field: SerializeReference, VerticalGroup(GroupName2)] public int MaxLevelUpgrade { get; private set; }
    [field: SerializeReference, VerticalGroup(GroupName2)] public TalentStat.Type StatGain { get; private set; }
    [field: SerializeReference, VerticalGroup(GroupName2)] public ScaleMethod StatScaleMethod { get; private set; }
    [field: SerializeReference, VerticalGroup(GroupName2)] public GameResource.Type ResourceRequire { get; private set; }
    [field: SerializeReference, VerticalGroup(GroupName2)] public ScaleMethod ResourceScaleMethod { get; private set; }
    [field: SerializeReference, VerticalGroup(GroupName2)] public string Description { get; private set; } = "";
    
    [field: SerializeField] public int[] RequireNode { get; private set; } = Array.Empty<int>();
    [field: SerializeField, TableList] public TalentTreeNodeLevelConfig[] TalentTreeNodeLevelConfigArray { get; private set; }
    public TalentTreeNodeLevelConfig GetTalentTreeNodeLevelConfig(int level)
    {
        if (level < 0 || level > MaxLevelUpgrade) return null;
        return TalentTreeNodeLevelConfigArray[level];
    }
#if UNITY_EDITOR
    [Button]
    private void UpdateConfig()
    {
        EditorUtility.SetDirty(this);
        TalentTreeNodeLevelConfigArray = new TalentTreeNodeLevelConfig[MaxLevelUpgrade + 1];
        for (int i = 0; i <= MaxLevelUpgrade; i++)
        {
            
            TalentStat talentStat = new TalentStat(StatGain, 0);
            GameResource gameResource = new GameResource(ResourceRequire, 0);
            if (i != 0)
            {
                talentStat.Amount = new BigNumber(StatScaleMethod.Evaluate(i));
                gameResource.Amount = new BigNumber(ResourceScaleMethod.Evaluate(i));
            }
            string description = string.Format(Description, talentStat.Amount.ToStringUI());
            
            TalentTreeNodeLevelConfigArray[i] = new TalentTreeNodeLevelConfig(i, talentStat, gameResource, description);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), $"TTNC_{NodeId}_{NodeName}");
    }
#endif
}

[System.Serializable]
public class TalentTreeNodeLevelConfig
{
    [field: TableColumnWidth(50, false)]
    [field: SerializeField] public int Level { get; set; }
    [field: TableColumnWidth(300, false)]
    [field: SerializeField] public TalentStat TalentStatGain {get; set;}
    [field: TableColumnWidth(200, false)]
    [field: SerializeField] public GameResource GameResourceRequire { get; set;}
    [field: SerializeField] public string Description {get; set;}

    public TalentTreeNodeLevelConfig(int level, TalentStat talentStatGain, GameResource gameResourceRequire, string description)
    {
        Level = level;
        TalentStatGain = talentStatGain;
        GameResourceRequire = gameResourceRequire;
        Description = description;
    }
}
[System.Serializable]
public class ScaleValue
{
    
}