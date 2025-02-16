using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.HeroAbility
{
    public class DoubleShotAbility : ActiveAbility
    {
        [field: SerializeField] public float DoubleShotRate {get; private set;}
        public override bool CanUseAbility()
        {
            return true;
        }

        public override void UseAbility()
        {
            
        }
    }
}