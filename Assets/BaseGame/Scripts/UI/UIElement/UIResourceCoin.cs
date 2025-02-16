using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using TMPro;
using TW.UGUI.MVPPattern;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.Extension;
using UnityEngine;

public class UIResourceCoin : ACachedMonoBehaviour, IAView
{
    [field: SerializeField] public TextMeshProUGUI TextCoin {get; private set;}
    [field: SerializeField] public Transform MainView {get; private set;}
    [field: SerializeField] public RectTransform ResourceImage {get; private set;}
    [field: SerializeField] public UIResourceEffect UIResourceEffect {get; private set;}
    private bool IsDelayIncrease { get; set; }
    private int DelayIncreaseValue { get; set; }
    private Vector3 DelayIncreaseValuePos { get; set; }
    private BigNumber StartResource { get; set; }
    private BigNumber TargetResource { get; set; }
    private BigNumber CurrentResource { get; set; }
    private MotionHandle TweenReMap { get; set; }

    public UniTask Initialize(Memory<object> args)
    {
        TargetResource = PlayerResourceData.Instance.GetGameResource(GameResource.Type.Coin).ReactiveAmount.Value;
        CurrentResource = TargetResource;
        PlayerResourceData.Instance.GetGameResource(GameResource.Type.Coin).ReactiveAmount.ReactiveProperty.Subscribe(OnCoinChange).AddTo(this);
        return UniTask.CompletedTask;
    }

    private void OnCoinChange(BigNumber coin)
    {
        if (IsDelayIncrease)
        {
            IsDelayIncrease = false;
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < DelayIncreaseValue; i++)
            {
                UIResourceEffect ui = Instantiate(UIResourceEffect, DelayIncreaseValuePos, Quaternion.identity, MainView);
                ui.Setup(DelayIncreaseValuePos, ResourceImage.position);
            }

            LMotion.Create(0, 1, 1).WithOnComplete(() => OnCoinChangeComplete(coin));
        }
        else
        {
            OnCoinChangeComplete(coin);
        }


    }
    private void OnCoinChangeComplete(BigNumber coin)
    {
        TargetResource = coin;
        StartResource = CurrentResource;
        TweenReMap.TryCancel();
        TweenReMap = LMotion.Create(0f, 1f, 0.5f).WithEase(Ease.Linear).Bind(OnUpdateResource);
    }

    private void OnUpdateResource(float process)
    {
        CurrentResource = ReMap(StartResource, TargetResource, process);
        TextCoin.SetText($"<style=l>{CurrentResource.RoundToInt().ToStringUI()}");
    }
    private BigNumber ReMap(BigNumber a, BigNumber b, float t)
    {
        return a + t * (b - a);
    }
    public void SetDelayIncreaseValue(Vector3 pos, int value)
    {
        IsDelayIncrease = true;
        DelayIncreaseValuePos = pos;
        DelayIncreaseValue = value;
    }
}
