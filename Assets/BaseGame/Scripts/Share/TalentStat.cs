using System;
using MemoryPack;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;

[MemoryPackable]
[Serializable]
public partial class TalentStat
{
    public enum Type
    {
        NormalEnemyDamageTaken = 0,
        NormalEnemyAmount = 1,
        NormalEnemyResourceDrop = 2,
        NormalEnemyExpDrop = 3, 

        
        RareEnemyDamageTaken = 10,
        RareEnemySpawnRate = 11,
        RareEnemyResourceDrop = 12,
        RareEnemyExpDrop = 13,
        RareEnemyAmount = 14,
        
        EpicEnemyDamageTaken = 20,
        EpicEnemySpawnRate = 21,
        EpicEnemyResourceDrop = 22,
        EpicEnemyExpDrop = 23,
        EpicEnemyAmount = 24,
        
        BossEnemyDamageTaken = 30,
        BossEnemySpawnRate = 31,
        BossEnemyResourceDrop = 32,
        BossEnemyExpDrop = 33,
        
        DoubleResourceDropRate = 40,
        DoubleExpDropRate = 41,
        
        DealDamageRegenPercent = 48,
        TakeDamageRegenPercent = 49,
        HitPoint = 50,
        HealthRegenWhenOutCombat = 51,
        RecoverySpeed= 52,
        AttackSpeedBoostAfterRecovery = 53,
        KillRegen = 54,
        DealDamageRegen = 55,
        TakeDamageRegen = 56,
        HitPointPerVitality = 57,
        HealthRegenWhenOutCombatPerVitality = 58,
        HealthRegenWhenShielded = 59,
        
        TakeDamageGainShield = 60,
        ShieldedAttackSpeed = 61,
        KillGainShield = 62,
        ShieldedHealthRegen = 63,
        RecoveryShield = 64,
        
        PhysicalAttackDamage = 70,
        AttackSpeed = 71,
        PhysicalCriticalChance = 72,
        PhysicalCriticalDamage = 73,
        DamageDealtToHighHpEnemy = 74,
        DamageDealtToLowHpEnemy = 75,
        PhysicalCriticalOverkillDamage = 76,
        PhysicalCriticalDamageBoostByHeroHp = 77,
        
        MagicalCriticalChance = 80,
        MagicalCriticalDamage = 81,
        
        
        DamageDealtToNormalEnemy = 90,
        DamageDealtToRareEnemy = 91,
        DamageDealtToEpicEnemy = 92,
        DamageDealtToBossEnemy = 93,
        
        FuryPoint = 100,
        ArcanePoint = 101,
        LuckPoint = 102,
        
        StrengthArrowRate = 110,
        StrengthArrowDamage = 111,
        StrengthArrowExecute = 112,
        
        SpiritArrowRate = 120,
        SpiritArrowDamage = 121,
        SpiritArrowMulti = 122,
        SpiritArrowCriticalDamage = 123,
        SpiritArrowConcentrationCriticalDamage = 124,
        SpiritArrowBossDamage = 125,
        SpiritArrowDoubleShot = 126,
        
        FireArrowRate = 130,
        FireArrowDamage = 131,
        FireArrowMulti = 132,
        FireArrowBurnAccuracyDamage = 133,
        
        ThunderArrowRate = 140,
        ThunderArrowDamage = 141,
        ThunderArrowCriticalDamage = 142,
        ThunderArrowDexterityCriticalDamage = 143,
        ThunderArrowParalysisDexterityDamage = 144,
        
        SpectralArrowRate = 150,
        SpectralArrowDamage = 151,
        SpectralArrowTripleShoot = 152,
        SpectralArrowIntelligentDamage = 153,
        
        SpecialArrowCriticalRate = 160,
        SpecialArrowCriticalDamage = 161,
        
        ConcentrationExpBonus = 200,
        StrengthExpBonus = 201,
        DexterityExpBonus = 202,
        AccuracyExpBonus = 203,
        VitalityExpBonus = 204,
        IntelligenceExpBonus = 205,
        
        SpeedFindingMonster = 300,
        
        
        
    }
    [MemoryPackIgnore]
    [field: HideLabel, HorizontalGroup(nameof(TalentStat), 200)]
    [field: SerializeField] public Type StatType { get; private set; }
    
    [MemoryPackIgnore]
    [field: SerializeField, HideLabel, HorizontalGroup(nameof(TalentStat))]
    public ReactiveValue<BigNumber> ReactiveAmount { get; private set; } = new();
    
    [MemoryPackIgnore]
    public BigNumber Amount
    {
        get => ReactiveAmount.Value;
        set => ReactiveAmount.Value = value;
    }
    
    
    public TalentStatData TalentStatData { get; set; } = new();

    [MemoryPackConstructor]
    public TalentStat()
    {
        
    }
    
    [MemoryPackOnSerializing]
    private void OnSerializing()
    {
        TalentStatData.StatType = StatType;
        TalentStatData.C = Amount.coefficient;
        TalentStatData.E = Amount.exponent;
    }
    
    [MemoryPackOnDeserialized]
    private void OnDeserialized()
    {
        StatType = TalentStatData.StatType;
        Amount = new BigNumber(TalentStatData.C, TalentStatData.E);
    }
    
    public TalentStat(Type statType, BigNumber amount)
    {
        StatType = statType;
        Amount = amount;
    }
    
    public TalentStatData ToTalentStatData()
    {
        return new TalentStatData()
        {
            StatType = StatType,
            C = Amount.coefficient,
            E = Amount.exponent,
        };
    }
    public TalentStat FromTalentStatData(TalentStatData talentStatData)
    {
        StatType = talentStatData.StatType;
        Amount = new BigNumber(talentStatData.C, talentStatData.E);
        return this;
    }
    public void Change(BigNumber value)
    {
        Amount += value;
    }
    public bool IsEnough(BigNumber value, float threshold = 0.001f)
    {
        return value <= Amount + threshold;
    }

}

[MemoryPackable]
[Serializable]
public partial class TalentStatData
{
    [field: SerializeField] public TalentStat.Type StatType { get; set; }
    [field: SerializeField] public double C { get; set; }
    [field: SerializeField] public int E { get; set; }
}