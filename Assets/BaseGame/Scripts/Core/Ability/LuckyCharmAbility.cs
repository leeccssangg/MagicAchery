using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.HeroAbility
{
    [CreateAssetMenu(fileName = "LuckyCharmAbility", menuName = "ScriptableObject/Ability/LuckyCharmAbility")]
    public class LuckyCharmAbility : ActiveCooldownAbility
    {
        [field: SerializeField] public AbilityValue<float> CoinGain {get; private set;}
        protected override AbilityValue<float> Value0 => Cooldown;
        protected override AbilityValue<float> Value1 => CoinGain;

        public override bool CanUseAbility()
        {
            return base.CanUseAbility();
        }

        public override void UseAbility()
        {
            base.UseAbility();
            PlayerResourceData.Instance.AddGameResource(GameResource.Type.Coin, CoinGain.GetValue(AbilityLevel));
        }
    }
}