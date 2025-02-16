using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TW.UGUI.Shared
{
    public class OffsetRectLocalMoveTransitionAnimation : IUnitTransitionAnimation
    {
        private enum OffsetType
        {
            Form,
            To,
        }
        public bool IsInitialized { get; set; }
        [field: SerializeField] private RectTransform Owner { get; set; }
        [field: SerializeField, HideInInspector] private OffsetType Offset {get; set;}
        
        [field: InlineButton(nameof(LockBeforeValue), SdfIconType.Lock, "")]
        [field: InlineButton(nameof(SetBeforeValue), SdfIconType.Download, "")]
        [field: HorizontalGroup("BeforeValue"), HideLabel]
        [field: ShowIf("@Offset == OffsetType.To")]
        [field: SerializeField]
        public Vector3 BeforeValue { get; set; } = Vector3.zero;

        [field: InlineButton(nameof(LockAfterValue), SdfIconType.Lock, "")]
        [field: InlineButton(nameof(SetAfterValue), SdfIconType.Download, "")]
        [field: HorizontalGroup("AfterValue"), HideLabel]
        [field: ShowIf("@Offset == OffsetType.Form")]
        [field: SerializeField]
        public Vector3 AfterValue { get; set; } = Vector3.zero;

        [field: HorizontalGroup("Time"), LabelWidth(80)]
        [field: SerializeField]
        public float Delay { get; set; }

        [field: HorizontalGroup("Time"), LabelWidth(80)]
        [field: SerializeField]
        public float Duration { get; set; } = 0.3f;

        [field: HorizontalGroup("Interpolate"), HideLabel]
        [field: SerializeField]
        public InterpolateTransition Interpolate { get; set; } = InterpolateTransition.Ease;

        [field: HorizontalGroup("Interpolate"), ShowIf("@Interpolate == InterpolateTransition.Ease")]
        [field: SerializeField, HideLabel]
        public Ease EaseType { get; set; } = Ease.Linear;

        [field: HorizontalGroup("Interpolate"), ShowIf("@Interpolate == InterpolateTransition.AnimationCurve")]
        [field: SerializeField, HideLabel]
        public AnimationCurve AnimationCurve { get; set; } = UnityEngine.AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

        public void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;
            if (Offset == OffsetType.Form)
            {
                BeforeValue = Owner.anchoredPosition;
            }
            else if (Offset == OffsetType.To)
            {
                AfterValue = Owner.anchoredPosition;
            }
        }

        public void Setup()
        {

        }

        public void SetTime(float time)
        {
            time = Mathf.Max(0.0f, time - Delay);
            float progress = Duration <= 0.0f ? 1.0f : Mathf.Clamp01(time / Duration);
            Vector3 currentValue = Vector3.zero;
            switch (Interpolate)
            {
                case InterpolateTransition.Ease:
                    currentValue = DOVirtual.EasedValue(BeforeValue, AfterValue, progress, EaseType);
                    break;
                case InterpolateTransition.AnimationCurve:
                    currentValue = DOVirtual.EasedValue(BeforeValue, AfterValue, progress, AnimationCurve);
                    break;
            }

            Owner.anchoredPosition = currentValue;
        }

        private void SetBeforeValue()
        {
            if (Owner == null) return;
            BeforeValue = Owner.anchoredPosition;
        }

        private void SetAfterValue()
        {
            if (Owner == null) return;
            AfterValue = Owner.anchoredPosition;
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