using System;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using UnityEngine;

public class UIAnimChest : ACachedMonoBehaviour
{
    [field: SerializeField] private FeelAnimation IdleAnimation { get; set; }
    [field: SerializeField] public FeelAnimation OpenAnimation { get; private set; }
    [field: SerializeField] public GameObject ChestGroup {get; private set;}
    [field: SerializeField] public Transform[] RewardContainer {get; private set;}
    public void SetActive(bool value)
    {
        ChestGroup.SetActive(value);
    }
    [Button]
    public void StartIdleAnim()
    {
        OpenAnimation.Stop();
        IdleAnimation.Play();
    }

    [Button]
    public void StartOpenAnim()
    {
        IdleAnimation.Stop();
        OpenAnimation.Play();
    }
    public void PlaySoundOpenChest()
    {
        //AudioManager.Instance.PlaySoundFx(AudioType.SfxOpenBox);
    }
}