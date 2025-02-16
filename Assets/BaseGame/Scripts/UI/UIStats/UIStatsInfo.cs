using System;
using DG.Tweening;
using LitMotion;
using MemoryPack;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using TW.ACacheEverything;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.Extension;
using UnityEngine;
using UnityEngine.UI;

public partial class UIStatsInfo : ACachedMonoBehaviour
{
    [field: SerializeField] public GameStat.Type StatType { get; private set; }
    [field: SerializeField] public StatUnlock StatUnlock { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtStatName { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtStatLevel { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtCostUnlock { get; private set; }
    [field: SerializeField] public GameObject GOSelected { get; private set; }
    [field: SerializeField] public GameObject GOUnlock { get; private set; }
    [field: SerializeField] public GameObject GOLock { get; private set; }
    [field: SerializeField] public Slider SliderExp { get; private set; }
    [field: SerializeField] public Button BtnSelectStat { get; private set; }
    [field: SerializeField] public Button BtnUnlock { get; private set; }
    private BigNumber CurrentLevel { get; set; }
    private BigNumber StartExp { get; set; }
    private BigNumber CurrentExp { get; set; }
    private BigNumber TotalExpChange { get; set; }
    private MotionHandle MotionHandle { get; set; }

    public void Setup(GameStat.Type statType)
    {
        StatType = statType;
        StatUnlock = StatsManager.Instance.GetStatsUnlock((int)StatType);
        CurrentLevel = PlayerStatData.Instance.GetGameResource(StatType).Level;
        StartExp = PlayerStatData.Instance.GetGameResource(StatType).ReactiveExperience.Value;
        CurrentExp = PlayerStatData.Instance.GetGameResource(StatType).ReactiveExperience.Value;
        TotalExpChange = 0;
        TxtStatName.SetText($"{StatType.ToString()}");
        TxtStatLevel.SetText($"Lv.{CurrentLevel.ToStringUI()}");
        SliderExp.value = (CurrentExp / GameStat.CalculateExperienceToLevel(CurrentLevel + 1)).ToFloat();

        InGameDataManager.Instance.InGameData.StatsData.UnlockedId.ReactiveProperty.Subscribe(OnStatUnlockedChange)
            .AddTo(this);
        //PlayerStatData.Instance.GetGameResource(statType).ReactiveExperience.ReactiveProperty
        //    .CombineLatest(PlayerStatData.Instance.GetGameResource(statType).ReactiveExperience.ReactiveProperty, (p, c) => (p, c))
        //    .Pairwise()
        //    .Subscribe(OnExpAndLevelChange).AddTo(this);

        PlayerStatData.Instance.GetGameResource(StatType).ReactiveExperience.ReactiveProperty.Pairwise()
            .Subscribe(OnExpChange).AddTo(this);
        PlayerBattleData.Instance.CurrentTrainingStatType.ReactiveProperty.Subscribe(OnTrainingStatTypeChange)
            .AddTo(this);
        PlayerResourceData.Instance.GetGameResource(GameResource.Type.Coin).ReactiveAmount.ReactiveProperty
            .Subscribe(OnCoinChange).AddTo(this);

        BtnSelectStat.SetOnClickDestination(OnClickBtnSelectStat);
        BtnUnlock.SetOnClickDestination(OnClickBtnUnlock);
    }

    private void OnStatUnlockedChange(int idUnlocked)
    {
        GOLock.SetActive(!StatsManager.Instance.IsStatsUnlocked(StatType)
                         && StatsManager.Instance.IsStatsUnlockNext(StatType));
        GOUnlock.SetActive(StatsManager.Instance.IsStatsUnlocked(StatType));
        TxtCostUnlock.SetText($"Unlock {StatsManager.Instance.GetCostUnlockNextStat().Amount.ToStringUI()} gold");
        BtnUnlock.interactable = StatsManager.Instance.IsStatsUnlockNext(StatType);
    }

    private void OnExpChange((BigNumber p, BigNumber c) value)
    {
        if (value.c < value.p) return;
        TotalExpChange = value.c - value.p;
        StartExp = CurrentExp;

        MotionHandle.TryCancel();
        MotionHandle = LMotion.Create(0f, 1f, 0.5f)
            .WithEase(LitMotion.Ease.OutCubic)
            .Bind(OnExpUpdateCache);
    }
    [ACacheMethod]
    private void OnExpUpdate(float value)
    {
                BigNumber expToNextLevel = GameStat.CalculateExperienceToLevel(CurrentLevel + 1);
                BigNumber currentExpChange = BigNumber.Lerp(0, TotalExpChange, value);
                CurrentExp = StartExp + currentExpChange;
                if (CurrentExp >= expToNextLevel)
                {
                    CurrentExp -= expToNextLevel;
                    StartExp -= expToNextLevel;
                    CurrentLevel += 1;
                    expToNextLevel = GameStat.CalculateExperienceToLevel(CurrentLevel + 1);
                    TxtStatLevel.SetText($"Lv.{CurrentLevel.ToStringUI()}");
                }

                SliderExp.value = (CurrentExp / expToNextLevel).ToFloat();
    }

    private void OnTrainingStatTypeChange(GameStat.Type value)
    {
        GOSelected.SetActive(value == StatType);
        BtnSelectStat.interactable = value != StatType;
    }

    private void OnClickBtnSelectStat(Unit _)
    {
        PlayerBattleData.Instance.CurrentTrainingStatType.Value = StatType;
        BtnSelectStat.interactable = false;
    }

    private void OnClickBtnUnlock(Unit _)
    {
        StatsManager.Instance.UnlockStat(StatUnlock.id);
        PlayerBattleData.Instance.CurrentTrainingStatType.Value = StatType;
        BtnUnlock.interactable = false;
        BtnSelectStat.interactable = false;
    }

    private void OnCoinChange(BigNumber value)
    {
        if (!StatsManager.Instance.IsStatsUnlockNext(StatType)) return;
        BtnUnlock.interactable = PlayerResourceData.Instance.IsEnoughGameResource(StatUnlock.cost);
    }
}