using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using UnityEngine;

public class StrengthArrow : Arrow
{
    protected override void OnMonsterHit(Monster monster)
    {
        BigNumber executeThreshold = TalentTreeManager.GetTalentStat(TalentStat.Type.StrengthArrowExecute).Amount;
        if (monster.CurrentHitPointPercent() <= executeThreshold/100f)
        {
            monster.InstanceDie();
        }
    }

    protected override BigNumber GetAttackDamageScale()
    {
        BigNumber damageScale = base.GetAttackDamageScale();
        damageScale += GetStrengthArrowDamageScale();
        return damageScale;
    }
    private BigNumber GetStrengthArrowDamageScale()
    {
        return TalentTreeManager.GetTalentStat(TalentStat.Type.StrengthArrowDamage).Amount / 100f;
    }
}