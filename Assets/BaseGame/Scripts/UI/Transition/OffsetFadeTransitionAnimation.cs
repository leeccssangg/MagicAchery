using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TW.UGUI.Shared
{
    public class OffsetFadeTransitionAnimation: IUnitTransitionAnimation
    {
        private enum OffsetType
        {
            Form,
            To,
        }
        public bool IsInitialized { get; set; }
        [field: SerializeField] private CanvasGroup Owner { get; set; }
        [field: SerializeField, HideInInspector] private OffsetType Offset {get; set;}
        
        [field: InlineButton(nameof(LockBeforeValue), SdfIconType.Lock, "")]
        [field: HorizontalGroup("BeforeValue"), HideLabel]
        [field: ShowIf("@Offset == OffsetType.To")]
        [field: SerializeField] 
        public float BeforeValue { get; set; } = 1;
        
        [field: InlineButton(nameof(LockAfterValue), SdfIconType.Lock, "")]
        [field: HorizontalGroup("AfterValue"), HideLabel]
        [field: ShowIf("@Offset == OffsetType.Form")]
        [field: SerializeField] 
        public float AfterValue { get; set; } = 1;


        [field: HorizontalGroup("Time"), LabelWidth(80)]
        [field: SerializeField] public float Delay { get; set; }
        [field: HorizontalGroup("Time"), LabelWidth(80)]
        [field: SerializeField] public float Duration { get; set; } = 0.3f;
        [field: HorizontalGroup("Interpolate"), HideLabel]
        [field: SerializeField] public InterpolateTransition Interpolate { get; set; } = InterpolateTransition.Ease;
        [field: HorizontalGroup("Interpolate"), ShowIf("@Interpolate == InterpolateTransition.Ease")]
        [field: SerializeField, HideLabel] public Ease EaseType { get; set; } = Ease.Linear;
        [field: HorizontalGroup("Interpolate"), ShowIf("@Interpolate == InterpolateTransition.AnimationCurve")]
        [field: SerializeField, HideLabel] public AnimationCurve AnimationCurve { get; set; } = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

        public void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;
            if (Offset == OffsetType.Form)
            {
                BeforeValue = Owner.alpha;
            }
            else if (Offset == OffsetType.To)
            {
                AfterValue = Owner.alpha;
            }
        }

        public void Setup()
        {

        }

        public void SetTime(float time)
        {
            time = Mathf.Max(0.0f, time - Delay);
            float progress = Duration <= 0.0f ? 1.0f : Mathf.Clamp01(time / Duration);
            float currentValue = 0;
            switch (Interpolate)
            {
                case InterpolateTransition.Ease:
                    currentValue = DOVirtual.EasedValue(BeforeValue, AfterValue, progress, EaseType);
                    break;
                case InterpolateTransition.AnimationCurve:
                    currentValue = DOVirtual.EasedValue(BeforeValue, AfterValue, progress, AnimationCurve);
                    break;
            }
            Owner.alpha = currentValue;
        }
        private void LockBeforeValue()
        {
            Offset = OffsetType.Form;
        }
        private void LockAfterValue()
        {
            Offset = OffsetType.To;
        }
    }
}