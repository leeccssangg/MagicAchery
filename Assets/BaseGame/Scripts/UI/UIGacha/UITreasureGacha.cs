using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TW.UGUI.Core.Sheets;
using TW.Utility.CustomComponent;
using TW.Utility.Extension;
using UnityEngine;
using UnityEngine.UI;

public class UITreasureGacha : ACachedMonoBehaviour
{
    [field: SerializeField] public GachaTreasureReward Reward { get; private set; }
    [field: SerializeField] public Image ImgIcon { get; private set; }
    [field: SerializeField] public Transform StartPos { get; private set; }
    [field: SerializeField] public Transform EndPos { get; private set; }
    [field: SerializeField] public Action ActionLastSummoned { get; private set; } = null;
    [field: SerializeField] public float FixedSpeed { get; private set; } = 500f;
    private Tween m_Tween;

    public void Setup(GachaTreasureReward reward, Transform startPos, Transform endPos)
    {
        Reward = reward;
        StartPos = startPos;
        EndPos = endPos;
        Transform.position = StartPos.position;
        SetupIconHero();
        //ImgIconHero.sprite = Resources.Load<Sprite>($"HeroIcon/{summonHeroReward.Config.Type}");
    }
    //public void SetupActionLastSummoned(Action action)
    //{
    //    ActionLastSummoned = action;
    //}
    public void InitDelay(float timeWait)
    {
        DOVirtual.DelayedCall(timeWait, StartMoving);
    }
    private void StartMoving()
    {
        this.Transform.DOMoveX(EndPos.position.x, Vector3.Distance(StartPos.position, EndPos.position)/400).From(StartPos.position).SetEase(Ease.Linear)
            .OnComplete
            (
                OnAnimCompleted
            );
    }
    private void SetupIconHero()
    {
        if (Reward.Config.Type != GachaTreasureType.Gem
            && Reward.Config.Type != GachaTreasureType.Gold 
            && Reward.Config.Type != GachaTreasureType.MythicStone
            )
        {
            //ImgIcon.sprite = Reward.RewardHero.SpriteIcon;
        }
    }
    private void OnAnimCompleted()
    {
        Debug.Log("MovingComplete");
        if (Reward.IsAcquired)
        {
            ModalGachaTreasureContext.Events.SpawnUIGachaReward?.Invoke(Reward);
            Debug.Log("SpawnUIGachaReward");
        }
        //ActionLastSummoned?.Invoke();
        ModalGachaTreasureContext.Events.CheckCompletedAnim?.Invoke();
        this.gameObject.SetActive(false);
        Debug.Log("Complete");
    }
}
