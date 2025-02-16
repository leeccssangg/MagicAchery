using System.Collections.Generic;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIToggleButton : MonoBehaviour
{
    [field: SerializeField] public Button MainButton { get; private set; }
    //[field: SerializeField] public GameObject[] ImageSwitch { get; private set; }
    [field: SerializeField] public GameObject[] ImageBackground { get; private set; }
    [field: SerializeField] public RectTransform Switch { get; private set; }
    [field: SerializeField] public bool Value { get; private set; }

    [field: SerializeField, Unity.Collections.ReadOnly]
    public Vector3 TargetSwitchPosition { get; private set; }

    public UnityEvent<bool> OnClickButton { get; private set; } = new UnityEvent<bool>();
    public List<Tween> AnimTween { get; private set; } = new List<Tween>();
    private bool IsInit { get; set; } = false;

    protected virtual void Awake()
    {
        if (MainButton == null)
        {
            MainButton = GetComponentInChildren<Button>();
        }

        Init();
    }

    protected virtual void Init()
    {
        if (IsInit) return;
        if (Switch is not null)
        {
            TargetSwitchPosition = Switch.localPosition;
        }

        MainButton.onClick.AddListener(() =>
        {
            Value = !Value;
            TargetSwitchPosition = new Vector3(-TargetSwitchPosition.x, TargetSwitchPosition.y, TargetSwitchPosition.z);
            AnimTween.ForEach(t => t.Kill());
            AnimTween.Clear();
            AnimTween.Add(Switch.DOLocalMove(TargetSwitchPosition, 0.2f));
            //ImageSwitch.ForEach((image, i) => image.SetActive(Value ? i == 0 : i == 1));
            ImageBackground.ForEach((image, i) => image.SetActive(Value ? i == 0 : i == 1));
            OnClickButton?.Invoke(Value);
        });
        IsInit = true;
    }

    public void SetupValue(bool value)
    {
        if (value == Value) return;
        if (!IsInit) Init();

        Value = !Value;
        TargetSwitchPosition = new Vector3(-TargetSwitchPosition.x, TargetSwitchPosition.y, TargetSwitchPosition.z);
        Switch.localPosition = TargetSwitchPosition;
        //ImageSwitch.ForEach((image, i) => image.SetActive(Value ? i == 0 : i == 1));
        ImageBackground.ForEach((image, i) => image.SetActive(Value ? i == 0 : i == 1));
    }
}