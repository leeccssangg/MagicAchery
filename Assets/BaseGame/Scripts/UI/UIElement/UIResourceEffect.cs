using System;
using System.Collections.Generic;
using DG.Tweening;
using TW.Utility.CustomComponent;
using UnityEngine;

public class UIResourceEffect : ACachedMonoBehaviour
{
    private List<Tween> TweenList { get; set; } = new List<Tween>();
    public void Setup(Vector3 startPos, Vector3 endPos)
    {

        Transform.localScale = Vector3.zero;
        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f), 0);
        float randomDelay = UnityEngine.Random.Range(0f, 0.5f);
        TweenList.Add(Transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetDelay(randomDelay));
        TweenList.Add(Transform.DOMove(startPos + randomPos, 0.5f).SetEase(Ease.InOutSine).SetDelay(randomDelay));

        TweenList.Add(Transform.DOMove(endPos, 0.5f).SetEase(Ease.InOutSine).OnComplete(SelfDespawn).SetDelay(randomDelay + 0.5f));
    }
    private void SelfDespawn()
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        TweenList.ForEach(tween => tween.Kill());
        TweenList.Clear();
    }
}