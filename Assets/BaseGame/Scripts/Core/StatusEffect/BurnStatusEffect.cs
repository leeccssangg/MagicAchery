using Core;
using TW.Utility.CustomType;
using TW.Utility.Extension;
using UnityEngine;

namespace Core.GameStatusEffect
{
    [System.Serializable]

    public class BurnStatusEffect : StatusEffect
    {
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float TickTime { get; private set; }
        private float DefaultTickTime { get; set; } = 0.2f;

        public BurnStatusEffect(float duration) : base(Type.Burn, false)
        {
            Duration = duration;
        }

        public override void Execute(IStatusEffectAble statusEffectAble)
        {
            base.Execute(statusEffectAble);
            if (statusEffectAble is not IBurnAble burnAble) return;
            if (statusEffectAble is not Monster monster) return;
            Duration -= Time.deltaTime;
            TickTime += Time.deltaTime;
                
            if (TickTime >= DefaultTickTime)
            {
                VisualEffect effect = FactoryManager.SpawnBurnEffect(monster.HitTransform.position,
                    Quaternion.identity, monster.HitTransform);
                effect.Transform.SetGlobalScale(Vector3.one);
                TalentStat fireArrowBurnAccuracyDamage =
                    TalentTreeManager.GetTalentStat(TalentStat.Type.FireArrowBurnAccuracyDamage);
                BigNumber attackDamage = 10 + PlayerStatData[GameStat.Type.Accuracy].Level *
                    fireArrowBurnAccuracyDamage.Amount / 100f;
                burnAble.TakeBurnDamage(attackDamage);
                TickTime -= DefaultTickTime;
            }
            
            if (Duration <= 0)
            {
                statusEffectAble.RemoveStatusEffect(this);
            }
        }

        public override void Overlap(IStatusEffectAble statusEffectAble, StatusEffect statusEffect)
        {
            base.Overlap(statusEffectAble, statusEffect);
            if (statusEffect is not BurnStatusEffect burnStatusEffect) return;
            Duration = Mathf.Max(Duration, burnStatusEffect.Duration);
        }
    }
}