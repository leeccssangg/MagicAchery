using System;
using UnityEngine;

namespace Core.GameStatusEffect
{
    [Serializable]
    public class StatusEffect
    {
        public enum Type
        {
            Paralysis,
            Burn,
            Stun,
            Fury,
            Poison,
        }

        private FactoryManager FactoryManagerCache { get; set; }
        protected FactoryManager FactoryManager => FactoryManagerCache ??= FactoryManager.Instance;
        private PlayerStatData PlayerStatDataCache { get; set; }
        protected PlayerStatData PlayerStatData => PlayerStatDataCache ??= PlayerStatData.Instance;
        private TalentTreeManager TalentTreeManagerCache { get; set; }
        protected TalentTreeManager TalentTreeManager => TalentTreeManagerCache ??= TalentTreeManager.Instance;


        [field: SerializeField] public Type StatusEffectType { get; private set; }
        [field: SerializeField] public bool IsStackable { get; private set; }

        public StatusEffect(Type statusEffectType, bool isStackable)
        {
            StatusEffectType = statusEffectType;
            IsStackable = isStackable;
        }

        public virtual void Overlap(IStatusEffectAble statusEffectAble, StatusEffect statusEffect)
        {

        }

        public virtual void OnAdd(IStatusEffectAble statusEffectAble)
        {

        }

        public virtual void Execute(IStatusEffectAble statusEffectAble)
        {

        }

        public virtual void OnRemove(IStatusEffectAble statusEffectAble)
        {

        }
    }
}