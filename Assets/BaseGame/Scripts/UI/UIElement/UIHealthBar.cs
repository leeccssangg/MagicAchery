using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using TW.Reactive.CustomComponent;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [field: SerializeField] public int MaxHealth {get; private set;}
    [field: SerializeField] public Slider CurrentHealthBar {get; private set;}
    [field: SerializeField] public Slider DelayCurrentHealthBar {get; private set;}
    [field: SerializeField] public TextMeshProUGUI TextHealLost {get; private set;}
    [field: SerializeField] public FeelAnimation HealLostAnimation {get; private set;}
    [field: SerializeField] public FeelAnimation WarningAnimation {get; private set;}
    private Tween CurrentTween {get; set;}
    private Tween DelayTween {get; set;}
    private Tween DelayWarningTween {get; set;}
    public void SetupMaxHealth(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealthBar.value = 0;
        DelayCurrentHealthBar.value = 0;
    }
    public void PlayHealthBarAnimation()
    {
        CurrentTween?.Kill();
        CurrentTween = CurrentHealthBar.DOValue(1, 0.1f).SetEase(Ease.Linear);
        DelayTween?.Kill();
        DelayTween = DelayCurrentHealthBar.DOValue(1, 0.5f).SetEase(Ease.Linear);
        DelayWarningTween?.Kill();
    }
    [Button]
    public void SetUpHealthBar(int lastHealth, int currentHealth)
    {
        WarningAnimation?.Stop();
        TextHealLost.text = (currentHealth - lastHealth).ToString();
        HealLostAnimation.Play();
        float currentHealthPercent = (float)currentHealth / MaxHealth;
        CurrentTween?.Kill();
        CurrentTween = CurrentHealthBar.DOValue(currentHealthPercent, 0.1f).SetEase(Ease.OutQuad);
        DelayTween?.Kill();
        DelayTween = DelayCurrentHealthBar.DOValue(currentHealthPercent, 0.5f).SetEase(Ease.Linear).SetDelay(1);
        if(currentHealthPercent <= 0.3f)
        {
            DelayWarningTween = DOVirtual.DelayedCall(1.55f, () => { WarningAnimation?.Play(); });
        }
    }
}