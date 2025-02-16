using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
 
namespace Core.HeroAbility
{
    [System.Serializable]
    public abstract class ActiveCooldownAbility : Ability
    {
        [field: SerializeField] public AbilityValue<float> Cooldown { get; set; }
        [ShowInInspector, ReadOnly] public bool IsOnCooldown { get; set; }
        [ShowInInspector, ReadOnly] public SerializableReactiveProperty<float> CooldownTimer { get; set; } = new(0);
        public float CooldownProgress => 1 - CooldownTimer.Value / Cooldown.GetValue(AbilityLevel);
        private CancellationTokenSource CooldownCancellationTokenSource { get; set; }
        private RadiusBar RadiusBar { get; set; }
        public Ability WithProgressBar(RadiusBar radiusBar)
        {
            RadiusBar = radiusBar;
            CooldownTimer.Subscribe(OnCooldownTimerChange).AddTo(TreasureOrb);
            return this;
        }
        public void OnCooldownTimerChange(float value)
        {
            RadiusBar.SetProgress(value/ Cooldown.GetValue(AbilityLevel));
        }
        public override Ability ResetAbility()
        {
            IsOnCooldown = true;
            CooldownTimer.Value = Cooldown.GetValue(AbilityLevel);
            return base.ResetAbility();
        }
        public void StartCooldownHandle()
        {
            IsOnCooldown = true;
            CooldownCancellationTokenSource = new CancellationTokenSource();
            ExecuteCooldown(CooldownCancellationTokenSource).Forget();
        }
        private async UniTask ExecuteCooldown(CancellationTokenSource cancellationTokenSource)
        {
            await foreach (AsyncUnit _ in UniTaskAsyncEnumerable.EveryUpdate()
                               .WithCancellation(cancellationTokenSource.Token))
            {
                if (CooldownTimer.Value <= 0)
                {
                    IsOnCooldown = false;
                    continue;
                }
                CooldownTimer.Value -= Time.deltaTime;
            }
        }
        public void StopCooldownHandle()
        {
            IsOnCooldown = false;
            CooldownCancellationTokenSource?.Cancel();
            CooldownCancellationTokenSource?.Dispose();
        }
        public void ResetCooldown()
        {
            IsOnCooldown = true;
            CooldownTimer.Value = Cooldown.GetValue(AbilityLevel);
        }
        public void ReduceCooldown(float rate)
        {
            CooldownTimer.Value -= Cooldown.GetValue(AbilityLevel) * rate / 100;
        }

        public override void OnAbilityStart()
        {
            base.OnAbilityStart();
            StartCooldownHandle();
        }
        public override void OnAbilityEnd()
        {
            base.OnAbilityEnd();
            StopCooldownHandle();
        }

        public override bool CanUseAbility()
        {
            return !IsOnCooldown;
        }

        public override void UseAbility()
        {
            ResetCooldown();
        }
    }
}