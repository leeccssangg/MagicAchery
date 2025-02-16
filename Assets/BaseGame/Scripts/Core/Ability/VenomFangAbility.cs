using System.Threading;
using Core.HeroAbility.AbilityTrigger;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core.HeroAbility
{
    [CreateAssetMenu(fileName = "VenomFangAbility", menuName = "ScriptableObject/Ability/VenomFangAbility")]
    public class VenomFangAbility : ActiveCooldownAbility, IAbilitySingleMonsterTarget
    {
        public BattleManager BattleManager => BattleManager.Instance;
        public Monster TargetMonster { get; set; }
        [field: SerializeField] public AbilityValue<float> DamageDeal { get; private set; }
        [field: SerializeField] public AbilityValue<float> Duration { get; private set; }
        [field: SerializeField] public Arrow Arrow { get; private set; }
        protected override AbilityValue<float> Value0 => DamageDeal;
        protected override AbilityValue<float> Value1 => Duration;

        public override bool CanUseAbility()
        {
            return base.CanUseAbility() && this.TryFindAnyMonsterTarget();
        }

        public override void UseAbility()
        {
            base.UseAbility();
            Arrow projectile = Arrow.Spawn(TreasureOrb.Transform.position, Quaternion.identity, TreasureOrb.Transform)
                .Setup(Owner, TreasureOrb.Transform.position, TargetMonster);
            BigNumber poisonDamage = DamageDeal.GetValue(AbilityLevel) * Owner.MagicalAttack.Current / 100;
            (projectile as VenomFangProjectile)?.WithPoison(poisonDamage, Duration.GetValue(AbilityLevel));
            projectile.Launch();
        }
    }
}