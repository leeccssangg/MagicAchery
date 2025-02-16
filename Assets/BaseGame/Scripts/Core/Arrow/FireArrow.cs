using Core.GameStatusEffect;
using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using UnityEngine;

public class FireArrow : Arrow
{
    protected override BigNumber GetAttackDamageScale()
    {
        BigNumber damageScale = base.GetAttackDamageScale();
        damageScale += GetFireArrowDamageScale();
        return damageScale;
    }
    private BigNumber GetFireArrowDamageScale()
    {
        BigNumber fireArrowDamage = TalentTreeManager.GetTalentStat(TalentStat.Type.FireArrowDamage).Amount;
        return fireArrowDamage/100f;
    }

    protected override void OnMonsterHit(Monster monster)
    {
        base.OnMonsterHit(monster);
        TalentStat fireArrowBurn = TalentTreeManager.GetTalentStat(TalentStat.Type.FireArrowBurnAccuracyDamage);
        if (fireArrowBurn.Amount == 0) return;
        monster.AddStatusEffect(new BurnStatusEffect(5));
    }
}