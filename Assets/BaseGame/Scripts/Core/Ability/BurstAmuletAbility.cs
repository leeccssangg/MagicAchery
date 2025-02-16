using Core.GameStatusEffect;
using UnityEngine;

namespace Core.HeroAbility
{
    [CreateAssetMenu(fileName = "BurstAmuletAbility", menuName = "ScriptableObject/Ability/BurstAmuletAbility")]
    public class BurstAmuletAbility : ActiveCooldownAbility
    {
        [field: SerializeField] public AbilityValue<float> AttackSpeedBuff {get; private set;}
        [field: SerializeField] public AbilityValue<float> BuffDuration {get; private set;}
        protected override AbilityValue<float> Value0 => AttackSpeedBuff;
        protected override AbilityValue<float> Value1 => BuffDuration;

        public override bool CanUseAbility()
        {
            return base.CanUseAbility() && Owner.IsInCombat();
        }

        public override void UseAbility()
        {
            base.UseAbility();
            Owner.AddStatusEffect(new AttackSpeedChangeStatusEffect(BuffDuration.GetValue(AbilityLevel), AttackSpeedBuff.GetValue(AbilityLevel)));
        }
    }
}