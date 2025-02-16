using System;
using Core.GameStatusEffect;
using Core.SimplePool;
using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using UnityEngine;
using Random = UnityEngine.Random;

public class ThunderArrow : Arrow
{
    protected override void OnMonsterHit(Monster monster)
    {
        base.OnMonsterHit(monster);
        TalentStat thunderArrowParalysis = TalentTreeManager.GetTalentStat(TalentStat.Type.ThunderArrowParalysisDexterityDamage);
        if (thunderArrowParalysis.Amount == 0) return;
        monster.AddStatusEffect(new ParalysisStatusEffect());
    }

    protected override BigNumber GetAttackDamageScale()
    {
        BigNumber damageScale = base.GetAttackDamageScale();
        damageScale += GetThunderArrowDamageScale();
        return damageScale;
    }
    protected override BigNumber GetCriticalDamageScale()
    {
        BigNumber damageScale = base.GetCriticalDamageScale();
        damageScale += GetThunderArrowCriticalDamageScale();
        damageScale += GetThunderArrowDexterityCriticalDamageScale();
        return damageScale;
    }
    private BigNumber GetThunderArrowDamageScale()
    {
        return TalentTreeManager.GetTalentStat(TalentStat.Type.ThunderArrowDamage).Amount / 100f;
    }
    private BigNumber GetThunderArrowCriticalDamageScale()
    {
        return TalentTreeManager.GetTalentStat(TalentStat.Type.ThunderArrowCriticalDamage).Amount / 100f;
    }
    private BigNumber GetThunderArrowDexterityCriticalDamageScale()
    {
        return TalentTreeManager.GetTalentStat(TalentStat.Type.ThunderArrowDexterityCriticalDamage).Amount / 100f;
    }
}