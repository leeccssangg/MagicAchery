using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core.AttributeShield
{
    [System.Serializable]
    public class Shield
    {
        private IShield Owner { get; set; }
        [field: SerializeField] private ProgressBar ShieldBar { get; set; }
        [field: SerializeField] public ReactiveValue<BigNumber> CurrentShield { get; set; }

        public void Init(IShield owner)
        {
            Owner = owner;
            CurrentShield = new ReactiveValue<BigNumber>(0);
            CurrentShield.ReactiveProperty.Subscribe(OnShieldChange).AddTo(Owner as Component);
        }

        public BigNumber GetDamageAfterShield(BigNumber damage)
        {
            if (!IsHavingShield()) return damage;
            if (IsEnoughShield(damage))
            {
                SetShieldValue(CurrentShield.Value - damage);
                return 0;
            }

            BigNumber damageAfterShield = damage - CurrentShield.Value;
            SetShieldValue(0);
            return damageAfterShield;
        }
        public void ChangeShieldValue(BigNumber value)
        {
            SetShieldValue(CurrentShield.Value + value);
        }
        public void SetShieldValue(BigNumber value)
        {
            CurrentShield.Value = BigNumber.Min(Owner.MaxShield, value);
        }
        public void ChangeShieldPercent(float percent)
        {
            ChangeShieldValue(Owner.MaxShield * percent);
        }
        public void SetShieldPercent(float percent)
        {
            BigNumber value = Owner.MaxShield * percent;
            SetShieldValue(value);
        }
        public bool IsEnoughShield(BigNumber value)
        {
            return CurrentShield.Value >= value;
        }
        public bool IsHavingShield()
        {
            return CurrentShield.Value > 0;
        }
        private void OnShieldChange(BigNumber shield)
        {
            float ratio = (shield / Owner.MaxShield).ToFloat();
            ShieldBar.SetProgress(ratio);
        }
    }
}