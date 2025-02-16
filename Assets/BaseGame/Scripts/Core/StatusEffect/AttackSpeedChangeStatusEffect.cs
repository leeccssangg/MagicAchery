using Core.AttributeAttackSpeed;
using UnityEngine;

namespace Core.GameStatusEffect
{
    [System.Serializable]
    public class AttackSpeedChangeStatusEffect : StatusEffect
    {
        [field: SerializeField] private float Duration { get; set; }
        [field: SerializeField] private float AttackSpeedBuff { get; set; }

        public AttackSpeedChangeStatusEffect(float duration, float attackSpeedBuff) : base(Type.Fury, true)
        {
            Duration = duration;
            AttackSpeedBuff = attackSpeedBuff;
        }

        public override void OnAdd(IStatusEffectAble statusEffectAble)
        {
            base.OnAdd(statusEffectAble);
            if (statusEffectAble is not IAttackSpeed attackSpeed) return;
            attackSpeed.AttackSpeed.ChangeOtherAttackSpeed(AttackSpeedBuff);
        }

        public override void Execute(IStatusEffectAble statusEffectAble)
        {
            base.Execute(statusEffectAble);
            if (statusEffectAble is not IAttackSpeed attackSpeed) return;
            Duration -= Time.deltaTime;
            if (Duration <= 0)
            {
                statusEffectAble.RemoveStatusEffect(this);
            }
        }

        public override void OnRemove(IStatusEffectAble statusEffectAble)
        {
            base.OnRemove(statusEffectAble);
            if (statusEffectAble is not IAttackSpeed attackSpeed) return;
            attackSpeed.AttackSpeed.ChangeOtherAttackSpeed(-AttackSpeedBuff);
        }
    }

}