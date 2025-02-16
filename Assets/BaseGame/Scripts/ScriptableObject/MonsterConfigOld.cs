using System;
using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "MonsterConfig", menuName = "ScriptableObject/MonsterConfig")]
public class MonsterConfigOld : ScriptableObject
{
    private const string GroupName1 = "MonsterConfig";
    private const string GroupName2 = "MonsterConfig/Stat";
    [HorizontalGroup(GroupName1, width: 100)]
    [PreviewField(ObjectFieldAlignment.Center, Height = 80), HideLabel, PropertyOrder(-1)]
    [ShowInInspector]
    public GameObject Preview => MonsterPrefab?.gameObject;
    [field: VerticalGroup(GroupName2)]
    [field: SerializeField] public Monster MonsterPrefab {get; private set;}
    [field: VerticalGroup(GroupName2)]
    [field: SerializeField] public Monster.Type MonsterType {get; private set;}
    [field: VerticalGroup(GroupName2)]
    [field: SerializeField] public int Map {get; private set;}
    [field: VerticalGroup(GroupName2)]
    [field: SerializeField] public int Level {get; private set;}
    [field: SerializeField] public BigNumber AttackDamage {get; private set;}
    [field: SerializeField] public float AttackSpeed {get; private set;}
    [field: SerializeField] public BigNumber HitPoint {get; private set;}
    [field: SerializeField] public BigNumber Experience {get; private set;}
    
#if UNITY_EDITOR
    [Button]
    private void FixName()
    {
        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), $"Config_{MonsterType.ToString()}_Monster_Map_{Map}_Level_{Level}");
        if (MonsterPrefab != null)
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(MonsterPrefab), $"{MonsterType.ToString()}_Monster_Map_{Map}_Level_{Level}");
        }
    }
    [Button]
    private void FixStat()
    {
        EditorUtility.SetDirty(this);
        int scale;
        switch (MonsterType)
        {
            case Monster.Type.Normal:
                scale = 1;
                break;
            case Monster.Type.Rare:
                scale = 3;
                break;
            case Monster.Type.Boss:
                scale = 5;
                break;
            default:
                scale = 1;
                break;
        }
        HitPoint = new BigNumber(24) * scale * new BigNumber(3).Pow((Map - 1) * 10 + Level) ;
        Experience = new BigNumber(12) * scale * new BigNumber(3).Pow((Map - 1) * 10 + Level);
    }
#endif
}

