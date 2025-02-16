using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using UnityEngine;

public class SpectralArrow : Arrow
{
    protected override BigNumber GetAttackDamageScale()
    {
        BigNumber damageScale = base.GetAttackDamageScale();
        damageScale += GetSpectralArrowDamageScale();
        return damageScale;
    }
    private BigNumber GetSpectralArrowDamageScale()
    {
        return TalentTreeManager.GetTalentStat(TalentStat.Type.SpectralArrowDamage).Amount / 100f;
    }
}