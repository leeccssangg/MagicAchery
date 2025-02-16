using UnityEngine;
using Sirenix.Utilities;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using Core.HeroAbility;
public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary,
    Mythic
}
public enum PlayerStats
{
    MagicDefense,
    Attack,
    HP,
    AttackSpeed,
    Armor,
    HPRegen,
    MagicDamage,
}

[CreateAssetMenu(fileName = "TreasurePoolGlobalConfig", menuName = "GlobalConfigs/TreasurePoolGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class TreasurePoolGlobalConfig : GlobalConfig<TreasurePoolGlobalConfig>
{
    public List<TreasureConfig> TreasureConfigs = new();
    public int NumSlot = 5;

    public TreasureConfig GetTreasureConfig(int id)
    {
        for(int i = 0; i < TreasureConfigs.Count; i++)
        {
            if(TreasureConfigs[i].Id == id)
            {
                return TreasureConfigs[i];
            }
        }
        return null;
    }
    public TreasureConfig GetRandomTreasureConfig()
    {
        return TreasureConfigs[Random.Range(0, TreasureConfigs.Count)];
    }
    public TreasureConfig GetRandomTreasureConfigByRarity(Rarity rarity)
    {
        List<TreasureConfig> treasureConfigs = new();
        for(int i = 0; i < TreasureConfigs.Count; i++)
        {
            if(TreasureConfigs[i].Rarity == rarity)
            {
                treasureConfigs.Add(TreasureConfigs[i]);
            }
        }
        return treasureConfigs[Random.Range(0, treasureConfigs.Count)];
    }
}
public class TreasureConfig : ScriptableObject
{
    public int Id;
    public string Name;
    public Rarity Rarity;
    public int FuryPoint;
    public int ArcanePoint;
    public int LuckPoint;
    public Ability Ability;
    public PlayerStats PlayerStats;
    public float MainStatBase;
    public float IncreasePerLevel;
    public float MainAbilityCooldown;
    public string DescriptionMainAbility = "";
    [ShowIf("@NeedValueMainAbility(0)")]
    public float MainAbilityValue0;
    [ShowIf("@NeedValueMainAbility(1)")]
    public float MainAbilityValue1;
    [ShowIf("@NeedValueMainAbility(2)")]
    public float MainAbilityValue2;
    [ShowIf("@NeedValueMainAbility(3)")]
    public float MainAbilityValue3;
    public string DescriptionUpgradeAbility6 = "";
    [ShowIf("@NeedValueUpgradeAbility6(0)")]
    public float UpgradeAbility6Value0;
    [ShowIf("@NeedValueUpgradeAbility6(1)")]
    public float UpgradeAbility6Value1;
    [ShowIf("@NeedValueUpgradeAbility6(2)")]
    public float UpgradeAbility6Value2;
    [ShowIf("@NeedValueUpgradeAbility6(3)")]
    public float UpgradeAbility6Value3;
    public string DescriptionUpgradeAbility12 = "";
    [ShowIf("@NeedValueUpgradeAbility12(0)")]
    public float UpgradeAbility12Value0;
    [ShowIf("@NeedValueUpgradeAbility12(1)")]
    public float UpgradeAbility12Value1;
    [ShowIf("@NeedValueUpgradeAbility12(2)")]
    public float UpgradeAbility12Value2;
    [ShowIf("@NeedValueUpgradeAbility12(3)")]
    public float UpgradeAbility12Value3;
    [ShowInInspector, ReadOnly]
    public string MainAbilityDescription => TryGetMainAbilityDescription();
    [ShowInInspector, ReadOnly]
    public string UpgradeAbility6Description => TryGetUpgradeAbility6Description();
    [ShowInInspector, ReadOnly]
    public string UpgradeAbility12Description => TryGetUpgradeAbility12Description();

    public string TryGetMainAbilityDescription()
    {
        try
        {
            return string.Format(DescriptionMainAbility, MainAbilityValue0, MainAbilityValue1, MainAbilityValue2, MainAbilityValue3);
        }
        catch (System.Exception)
        {

            return "";
        }
    }
    public string TryGetUpgradeAbility6Description()
    {
        try
        {
            return string.Format(DescriptionUpgradeAbility6, UpgradeAbility6Value0, UpgradeAbility6Value1, UpgradeAbility6Value2, UpgradeAbility6Value3);
        }
        catch (System.Exception)
        {

            return "";
        }
    }
    public string TryGetUpgradeAbility12Description()
    {
        try
        {
            return string.Format(DescriptionUpgradeAbility12, UpgradeAbility12Value0, UpgradeAbility12Value1, UpgradeAbility12Value2, UpgradeAbility12Value3);
        }
        catch (System.Exception)
        {

            return "";
        }
    }
    public virtual void UseAbility()
    {
        Debug.Log("UseAbility");
    }
    public virtual void IsUseableAbility()
    {
        Debug.Log("IsUseableAbility");
    }
#if UNITY_EDITOR
    public bool NeedValueMainAbility(int index)
    {
        return DescriptionMainAbility.Contains("{" + index + "}");
    }
    public bool NeedValueUpgradeAbility6(int index)
    {
        return DescriptionUpgradeAbility6.Contains("{" + index + "}");
    }
    public bool NeedValueUpgradeAbility12(int index)
    {
        return DescriptionUpgradeAbility12.Contains("{" + index + "}");
    }
#endif
}