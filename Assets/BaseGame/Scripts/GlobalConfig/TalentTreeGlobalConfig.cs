using System;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TalentTreeGlobalConfig", menuName = "GlobalConfigs/TalentTreeGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class TalentTreeGlobalConfig : GlobalConfig<TalentTreeGlobalConfig>
{
    [field: SerializeField] public TalentTreeNodeConfig[] TalentTreeNodeConfigs { get; private set; } = Array.Empty<TalentTreeNodeConfig>();

    public TalentTreeNodeConfig[] GetAllTalentTreeNodeConfigs()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        TalentTreeNodeConfigs = AssetDatabase.FindAssets("t:TalentTreeNodeConfig")
            .Select(guid => AssetDatabase.LoadAssetAtPath<TalentTreeNodeConfig>(AssetDatabase.GUIDToAssetPath(guid)))
            .ToArray();
        
        
#endif
        return TalentTreeNodeConfigs;
    }
}