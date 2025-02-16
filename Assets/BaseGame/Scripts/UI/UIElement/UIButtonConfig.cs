using System;
using DG.Tweening;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonConfig : ACachedMonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Button m_Button;
    private Button Button
    {
        get
        {
            if (m_Button == null)
            {
                m_Button = GetComponent<Button>();
            }
            return m_Button;
        }
    }
    
    private Tween AnimTween { get;set; }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Button.interactable == false) return;
        AnimTween?.Kill();
        AnimTween = Transform.DOScale(0.95f, 0.15f);
        //AudioManager.Instance.PlaySoundFx(AudioType.SfxUIClickBtn);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Button.interactable == false) return;
        AnimTween?.Kill();
        AnimTween = Transform.DOScale(1f, 0.15f);
    }

    private void OnDestroy()
    {
        AnimTween?.Kill();
    }
}