using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using UnityEngine;

public class SpiritArrow : Arrow
{
    protected override BigNumber GetAttackDamageScale()
    {
        BigNumber damageScale = base.GetAttackDamageScale();
        damageScale += GetSpiritArrowDamageScale();
        damageScale += GetSpiritArrowBossDamageScale();
        return damageScale;
    }

    protected override BigNumber GetCriticalDamageScale()
    {
        BigNumber damageScale = base.GetCriticalDamageScale();
        damageScale += GetSpiritArrowCriticalDamageScale();
        damageScale += GetSpiritArrowConcentrationCriticalDamageScale();
        return damageScale;
    }
    private BigNumber GetSpiritArrowDamageScale()
    {
        return TalentTreeManager.GetTalentStat(TalentStat.Type.SpiritArrowDamage).Amount / 100f;
    }
    private BigNumber GetSpiritArrowBossDamageScale()
    {
        if (TargetMonster.MonsterType != Monster.Type.Boss) return 0;
        return TalentTreeManager.GetTalentStat(TalentStat.Type.SpiritArrowBossDamage).Amount / 100f;
    }
    private BigNumber GetSpiritArrowCriticalDamageScale()
    {
        return TalentTreeManager.GetTalentStat(TalentStat.Type.SpiritArrowCriticalDamage).Amount / 100f;
    }
    private BigNumber GetSpiritArrowConcentrationCriticalDamageScale()
    {
        return TalentTreeManager.GetTalentStat(TalentStat.Type.SpiritArrowConcentrationCriticalDamage).Amount / 100f;
    }
}