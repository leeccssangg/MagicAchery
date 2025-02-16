using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using UnityEngine;

namespace Core
{
    public class VisualEffect : ACachedMonoBehaviour, IPoolAble<VisualEffect>
    {
        [field: SerializeField, SuffixLabel("millisecond ", true)]
        public int SelfDespawnTime { get; private set; }
        public virtual VisualEffect WithSpeed(float speed)
        {
            
            return this;
        }
        public virtual VisualEffect OnSpawn()
        {
            SelfDespawn().Forget();
            return this;
        }

        public virtual void OnDespawn()
        {

        }

        private async UniTask SelfDespawn()
        {
            await UniTask.Delay(SelfDespawnTime, cancellationToken: this.GetCancellationTokenOnDestroy());
            this.Despawn();
        }
    }
}