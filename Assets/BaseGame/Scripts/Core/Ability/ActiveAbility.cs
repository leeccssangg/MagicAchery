using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.HeroAbility
{
    [System.Serializable]
    public abstract class ActiveAbility : Ability
    {

        protected UniTask DelaySample(int sample, CancellationToken ct)
        {
            int timeDelay = (int)(1000 / Owner.AttackSpeed.Current * sample / 30);
            return UniTask.Delay(timeDelay, cancellationToken: ct);
        }
    }
}