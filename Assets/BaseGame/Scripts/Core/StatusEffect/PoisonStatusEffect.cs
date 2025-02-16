using TW.Utility.CustomType;
using UnityEngine;

namespace Core.GameStatusEffect
{
    public class PoisonStatusEffect : StatusEffect
    {
        [field: SerializeField] public BigNumber Damage {get; private set;}
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float TickTime { get; private set; }
        private float DefaultTickTime { get; set; } = 0.2f;
        public PoisonStatusEffect(BigNumber damage, float duration) : base(Type.Poison, true)
        {
            Damage = damage;
            Duration = duration;
        }
        public override void Execute(IStatusEffectAble statusEffectAble)
        {
            base.Execute(statusEffectAble);
            if (statusEffectAble is not IPoisonAble poisonAble) return;
            if (statusEffectAble is not Monster monster) return;
            Duration -= Time.deltaTime;
            TickTime += Time.deltaTime;
                
            if (TickTime >= DefaultTickTime)
            {
                // VisualEffect effect = FactoryManager.SpawnBurnEffect(monster.HitTransform.position,
                //     Quaternion.identity, monster.HitTransform);
                // effect.Transform.SetGlobalScale(Vector3.one);
                poisonAble.TakePoisonDamage(Damage);
                TickTime -= DefaultTickTime;
            }
            
            if (Duration <= 0)
            {
                statusEffectAble.RemoveStatusEffect(this);
            }
        }
    }
}