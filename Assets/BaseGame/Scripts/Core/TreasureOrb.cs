using System;
using System.Threading;
using Core.HeroAbility;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using UnityEngine;

namespace Core
{
    public class TreasureOrb : ACachedMonoBehaviour
    {
        [field: SerializeField] public Ability AbilityConfig {get; private set;}
        [field: SerializeField] public Hero Hero {get; private set;}
        [field: SerializeField] public Ability Ability {get; private set;}
        [field: SerializeField] public RadiusBar RadiusBar {get; private set;}

        private void Start()
        {
            InitAbility();
        }

        private void OnDestroy()
        {
            Ability.OnAbilityEnd();
        }

        private void Update()
        {
            if (Ability.CanUseAbility())
            {
                Ability.UseAbility();
            }
        }

        public void Setup(Hero hero, Ability ability)
        {
            Hero = hero;
            Ability = ability;
        }
        public void InitAbility()
        {
            Ability = AbilityConfig.Initialize(Hero, this);

            RadiusBar.SetActive(Ability is ActiveCooldownAbility);
            if (Ability is ActiveCooldownAbility activeCooldownAbility)
            {
                activeCooldownAbility.WithProgressBar(RadiusBar);
            }
            
            Ability.OnAbilityStart();
        }
    }
}