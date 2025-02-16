using R3;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core.AttributeHitPoint
{
    [System.Serializable]
    public class HitPoint
    {
        private PlayerStatData PlayerStatData => PlayerStatData.Instance;
        private TalentTreeManager TalentTreeManager => TalentTreeManager.Instance;
        private IHitPoint Owner { get; set; }
        [field: SerializeField] private ProgressBar ProgressBar { get; set; }
        [field: SerializeField] public BigNumber Base {get; private set;}
        [field: SerializeField] public ReactiveValue<BigNumber> Current { get; set; }
        [field: SerializeField] public BigNumber Max { get; private set; }
        public BigNumber VitalityHitPoint { get; set; }
        public BigNumber TalentHitPoint { get; set; }
        public float CurrentPercent => (Current.Value / Max).ToFloat();
        public bool IsFull => Current.Value >= Max; 
        public bool IsExhausted => Current.Value <= 0;
        public void Init(IHitPoint owner)
        {
            Owner = owner;
            Current = new ReactiveValue<BigNumber>(Base);
            Max = new ReactiveValue<BigNumber>(Base);
            
            Current.ReactiveProperty.Subscribe(OnCurrentHitPointChange).AddTo(Owner as Component);
            
            PlayerStatData[GameStat.Type.Vitality].ReactiveLevel.ReactiveProperty
                .Subscribe(OnVitalityLevelChange)
                .AddTo(Owner as Component);
            TalentTreeManager.GetTalentStat(TalentStat.Type.HitPoint).ReactiveAmount.ReactiveProperty
                .Subscribe(OnTalentHitPointChange)
                .AddTo(Owner as Component);
        }

        private void OnVitalityLevelChange(BigNumber vitalityLevel)
        {
            VitalityHitPoint = vitalityLevel * 10;
            float percent = CurrentPercent;
            Max = Base + VitalityHitPoint + TalentHitPoint;
            Current.Value = Max * percent;
        }
        private void OnTalentHitPointChange(BigNumber talentHitPoint)
        {
            TalentHitPoint = talentHitPoint;
            float percent = CurrentPercent;
            Max = Base + VitalityHitPoint + TalentHitPoint;
            Current.Value = Max * percent;
        }
        
        private void OnCurrentHitPointChange(BigNumber currentHitPoint)
        {
            ProgressBar.SetProgress((currentHitPoint / Max).ToFloat());
        }
        
        public void SetMaxHitPoint(BigNumber maxHitPoint)
        {
            Max = maxHitPoint;
        }
        public void SetCurrentHitPoint(BigNumber currentHitPoint)
        {
            Current.Value = currentHitPoint;
        }
        public void ChangeMaxHitPoint(BigNumber changeMaxHitPoint)
        {
            Max += changeMaxHitPoint;
        }
        public void ChangeHitPoint(BigNumber changeHitPoint)
        {
            Current.Value += changeHitPoint;
        }
        public void ChangeHitPointPercent(float percent)
        {
            Current.Value += Max * percent;
        }
        public void Recover(float deltaTime)
        {
            if (IsFull) return;
            float recoverSpeed = TalentTreeManager.GetTalentStat(TalentStat.Type.RecoverySpeed).Amount.ToFloat() /100f;
            ChangeHitPointPercent(0.1f * (1 + recoverSpeed) * deltaTime);
        }

    }
}