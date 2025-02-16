using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using TW.Utility.CustomType;
using UnityEngine;
using R3;
namespace Core.AttributeHeal
{
    [System.Serializable]
    public class Heal
    {
        private PlayerStatData PlayerStatData => PlayerStatData.Instance;
        private TalentTreeManager TalentTreeManager => TalentTreeManager.Instance;
        private IHeal Owner { get; set; }
        [field: SerializeField] public BigNumber RegenWhenShielded { get; set; }
        [field: SerializeField] public  BigNumber RegenWhenNotShielded { get; set; }
        [field: SerializeField] public  BigNumber RegenWhenInCombat { get; set; }
        [field: SerializeField] public BigNumber RegenWhenOutCombat { get; set; }
        [field: SerializeField] public BigNumber RegenWhenOutCombatPerVitality { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }
        public void Init(IHeal owner)
        {
            Owner = owner;
            RegenWhenShielded = BigNumber.ZERO;
            RegenWhenNotShielded = BigNumber.ZERO;
            RegenWhenInCombat = BigNumber.ZERO;
            RegenWhenOutCombat = BigNumber.ZERO;
            RegenWhenOutCombatPerVitality = BigNumber.ZERO;
            
            TalentTreeManager.GetTalentStat(TalentStat.Type.HealthRegenWhenOutCombat)
                .ReactiveAmount.ReactiveProperty
                .Subscribe(OnTalentTreeHealthRegenWhenOutCombatChange).AddTo(Owner as Component);
            TalentTreeManager.GetTalentStat(TalentStat.Type.HealthRegenWhenOutCombatPerVitality)
                .ReactiveAmount.ReactiveProperty
                .Subscribe(OnTalentTreeHealthRegenWhenOutCombatPerVitalityChange).AddTo(Owner as Component);
            TalentTreeManager.GetTalentStat(TalentStat.Type.HealthRegenWhenShielded)
                .ReactiveAmount.ReactiveProperty
                .Subscribe(OnTalentTreeHealthRegenWhenShieldedChange).AddTo(Owner as Component);
        }
        private void OnTalentTreeHealthRegenWhenOutCombatChange(BigNumber talentAmount)
        {
            RegenWhenOutCombat = talentAmount;
        }
        private void OnTalentTreeHealthRegenWhenOutCombatPerVitalityChange(BigNumber talentAmount)
        {
            RegenWhenOutCombatPerVitality = talentAmount;
        }
        private void OnTalentTreeHealthRegenWhenShieldedChange(BigNumber talentAmount)
        {
            RegenWhenShielded = talentAmount;
        }
        public void StartHeal()
        {
            HealHandler().Forget();
        }

        public void StopHeal()
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;
        }
        public async UniTask HealHandler()
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = new CancellationTokenSource();
            await foreach (AsyncUnit _ in UniTaskAsyncEnumerable.EveryUpdate()
                               .WithCancellation(CancellationTokenSource.Token))
            {
                BigNumber healAmount = BigNumber.ZERO;
                healAmount += Owner.IsShielded() ? RegenWhenShielded : RegenWhenNotShielded;
                healAmount += Owner.IsInCombat() ? RegenWhenInCombat : RegenWhenOutCombat + RegenWhenOutCombatPerVitality * PlayerStatData[GameStat.Type.Vitality].ReactiveLevel.Value;
                Owner.TakeHeal(healAmount);
            }
        }
    }
}