using System;
using System.Collections;
using System.Collections.Generic;
using TW.UGUI.Shared;
using TW.UGUI.Utility;
using UnityEngine;
using UnityEngine.UI;

public class UIShopTabButton : MonoBehaviour
{
    [field: SerializeField] public Button MainButton {get; private set;}
    [field: SerializeField] public TransitionAnimationBehaviour SelectAnim {get; private set;}
    [field: SerializeField] public TransitionAnimationBehaviour DeselectAnim {get; private set;}
    public Action OnSelect { get; set; }
    private bool IsSelected { get; set; }
    private void Awake()
    {
        MainButton.onClick.AddListener(OnMainButtonClicked);
    }
    
    private void OnMainButtonClicked()
    {
        OnSelect?.Invoke();
    }

    public void OnDeselectTab()
    {
        if (!IsSelected) return;
        IsSelected = false;
        SelectAnim.Stop();
        DeselectAnim.PlayAsync();
    }
    public void OnSelectTab()
    {
        if (IsSelected) return;
        IsSelected = true;
        DeselectAnim.Stop();
        SelectAnim.PlayAsync();
    }
}
