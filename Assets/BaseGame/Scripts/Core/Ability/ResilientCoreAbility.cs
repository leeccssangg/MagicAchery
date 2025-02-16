using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.HeroAbility
{
    [CreateAssetMenu(fileName = "ResilientCoreAbility", menuName = "ScriptableObject/Ability/ResilientCoreAbility")]
    public class ResilientCoreAbility : ActiveAbility
    {
        [field: SerializeField] public AbilityValue<float> AttackCount {get; private set;}
        [field: SerializeField] public AbilityValue<float> HealGain {get; private set;}
        [ShowInInspector] public int CurrentAttackCount {get; private set;}
        protected override AbilityValue<float> Value0 => AttackCount;
        protected override AbilityValue<float> Value1 => HealGain;

        public override void OnAbilityStart()
        {
            base.OnAbilityStart();
            Owner.OnTakeDamageCallback += IncreaseAttackCount;
        }
        public void IncreaseAttackCount()
        {
            CurrentAttackCount++;
        }

        public override bool CanUseAbility()
        {
            return CurrentAttackCount >= AttackCount.GetValue(AbilityLevel);
        }

        public override void UseAbility()
        {
            Owner.HitPoint.ChangeHitPointPercent(HealGain.GetValue(AbilityLevel)/100f);
            CurrentAttackCount -= Mathf.RoundToInt(AttackCount.GetValue(AbilityLevel));
        }
        
    }
}