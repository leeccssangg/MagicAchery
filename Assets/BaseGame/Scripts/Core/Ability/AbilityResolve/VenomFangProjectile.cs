using Core.GameStatusEffect;
using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using UnityEngine;

public class VenomFangProjectile : Arrow
{
    [field: SerializeField] public BigNumber PoisonDamage {get; private set;}
    [field: SerializeField] public float PoisonDuration {get; private set;}
    protected override BigNumber GetDamageDeal(Monster monster, out bool isCritical)
    {
        isCritical = false;
        return BigNumber.ZERO;
    }
    public VenomFangProjectile WithPoison(BigNumber poisonDamage, float poisonDuration)
    {
        PoisonDamage = poisonDamage;
        PoisonDuration = poisonDuration;
        return this;
    }


    protected override void OnMonsterHit(Monster monster)
    {
        base.OnMonsterHit(monster);
        monster.AddStatusEffect(new PoisonStatusEffect(PoisonDamage, PoisonDuration));
    }
}