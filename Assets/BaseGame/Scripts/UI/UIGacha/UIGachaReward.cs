using System.Collections;
using System.Collections.Generic;
using TMPro;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.UI;
public class UIGachaReward : ACachedMonoBehaviour
{
    [field: SerializeField] public GachaTreasureReward Reward { get; private set; }
    [field: SerializeField] public Image ImgReward { get; private set; }
    [field: SerializeField] public Image ImgRewardBg { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtRewardAmout { get; private set; }
    [field: SerializeField] public FeelAnimation AnimOff { get; private set; }
    [field: SerializeField] public FeelAnimation AnimOn { get; private set; }

    public void Setup(GachaTreasureReward reward)
    {
        Reward = reward;
        TxtRewardAmout.SetText($"{Reward.Config.RewardAmount * Reward.NumGacha}");
        SetupIconHero();
    }
    private void SetupIconHero()
    {
        if (Reward.Config.Type != GachaTreasureType.Gold
            && Reward.Config.Type != GachaTreasureType.Gem
            && Reward.Config.Type != GachaTreasureType.MythicStone)
        {
            //ImgReward.sprite = Reward.RewardHero.SpriteIcon;
        }
    }
    public void PlayAnim(bool isOn)
    {
        if (isOn)
        {
            AnimOn.Play();
        }
        else
        {
            AnimOff.Play();
        }
    }
}
