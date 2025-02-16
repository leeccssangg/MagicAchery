using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace  Core.HeroAbility
{
    [InlineEditor]
    [Serializable]
    public abstract class Ability : ScriptableObject
    {   
        [field: SerializeField] public string AbilityName {get; private set;}
        [field: SerializeField] public int AbilityLevel {get; private set;}
        [field: SerializeField] public string AbilityDescriptionFormat {get; private set;} = "";
        public Hero Owner {get; private set;}
        public TreasureOrb TreasureOrb {get; private set;}
        protected virtual AbilityValue<float> Value0 {get;} = new();
        protected virtual AbilityValue<float> Value1 {get;} = new();
        protected virtual AbilityValue<float> Value2 {get;} = new();
        public virtual Ability WithOwnerHero(Hero owner)
        {
            Owner = owner;
            return this;
        }
        public virtual Ability WithTreasureOrb(TreasureOrb treasureOrb)
        {
            TreasureOrb = treasureOrb;
            return this;
        }
        public virtual Ability ResetAbility()
        {
            return this;
        }
        public Ability Create()
        {
            return Instantiate(this);
        }
        public Ability Initialize(Hero owner, TreasureOrb treasureOrb)
        {
            return Create().WithOwnerHero(owner).WithTreasureOrb(treasureOrb);
        }
        [ShowInInspector]
        public string AbilityDescription
        {
            get
            {
                try
                {
                    return string.Format(AbilityDescriptionFormat, 
                        Value0.GetValue(AbilityLevel),
                        Value1.GetValue(AbilityLevel),
                        Value2.GetValue(AbilityLevel));
                }
                catch (Exception _)
                {
                    return "";
                }

            }
        }
        public virtual void OnAbilityStart()
        {
        }
        public virtual void OnAbilityEnd()
        {
        }
        public abstract bool CanUseAbility();
        public abstract void UseAbility();
    }
}