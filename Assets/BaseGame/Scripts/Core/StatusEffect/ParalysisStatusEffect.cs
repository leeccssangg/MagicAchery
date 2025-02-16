using Core;
using TW.Utility.CustomType;
using TW.Utility.Extension;
using UnityEngine;

namespace Core.GameStatusEffect
{
    [System.Serializable]
    public class ParalysisStatusEffect : StatusEffect
    {
        public ParalysisStatusEffect() : base(Type.Paralysis, true)
        {
            
        }

        public override void OnAdd(IStatusEffectAble statusEffectAble)
        {
            base.OnAdd(statusEffectAble);
            if (statusEffectAble is not IParalysisAble paralysisAble) return;
            if (statusEffectAble is not Monster monster) return;
            paralysisAble.ParalysisStack++;
            if (paralysisAble.ParalysisStack >= 3)
            {
                VisualEffect effect = FactoryManager.SpawnParalysisEffect(monster.HitTransform.position,
                    Quaternion.identity, monster.HitTransform);
                effect.Transform.SetGlobalScale(Vector3.one);
                TalentStat thunderArrowParalysis =
                    TalentTreeManager.GetTalentStat(TalentStat.Type.ThunderArrowParalysisDexterityDamage);
                BigNumber attackDamage = 10 + PlayerStatData[GameStat.Type.Dexterity].Level *
                    thunderArrowParalysis.Amount / 100f;
                paralysisAble.TakeParalysisDamage(attackDamage);
                paralysisAble.ParalysisStack -= 3;
            }
            statusEffectAble.RemoveStatusEffect(this);
        }
    }
}