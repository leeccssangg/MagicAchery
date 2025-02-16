using System;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using R3;
public class TreasureManager : Singleton<TreasureManager>
{
    [field: SerializeField] public TreasureUpgradeData UpgradeData { get; private set; }
    [field: SerializeField] public TreasureSlotData SlotData { get; private set; }
    private TreasurePoolGlobalConfig TreasurePoolGlobalConfig => TreasurePoolGlobalConfig.Instance;
    private TreasureUpgradeGlobalConfig TreasureUpgradeGlobalConfig => TreasureUpgradeGlobalConfig.Instance;
    private TreasureGachaGlobalConfig TreasureGachaGlobalConfig => TreasureGachaGlobalConfig.Instance;

    [field: SerializeField] public ReactiveValue<int> CurStageGacha { get; private set; }

    [field: SerializeField] public ReactiveValue<BigNumber> FuryPoint { get; private set; }
    [field: SerializeField] public ReactiveValue<BigNumber> ArcanePoint { get; private set; }
    [field: SerializeField] public ReactiveValue<BigNumber> LuckPoint { get; private set; }
    [field: SerializeField] public ReactiveValue<BigNumber> RemainFuryPoint { get; private set; } = new(0);
    [field: SerializeField] public ReactiveValue<BigNumber> RemainArcanePoint { get; private set; } = new(0);
    [field: SerializeField] public ReactiveValue<BigNumber> RemainLuckPoint { get; private set; } = new(0);

    #region Unity
    private void Start()
    {
        LoadData();
    }
    #endregion

    #region Save & Load
    private void LoadData()
    {
        FuryPoint = TalentTreeManager.Instance.GetTalentStat(TalentStat.Type.FuryPoint).ReactiveAmount;
        ArcanePoint = TalentTreeManager.Instance.GetTalentStat(TalentStat.Type.ArcanePoint).ReactiveAmount;
        LuckPoint = TalentTreeManager.Instance.GetTalentStat(TalentStat.Type.LuckPoint).ReactiveAmount;
        UpgradeData = InGameDataManager.Instance.InGameData.TreasureData.TreasureUpgradeData;
        SlotData = InGameDataManager.Instance.InGameData.TreasureData.TreasureSlotData;
        if(UpgradeData.Data.Count <= 0)
        {
            InitNewData();
        }
        FuryPoint.ReactiveProperty.Subscribe( _ => UpdateRemainPoint()  ).AddTo(this);
        ArcanePoint.ReactiveProperty.Subscribe(_ => UpdateRemainPoint()).AddTo(this);
        LuckPoint.ReactiveProperty.Subscribe(_ => UpdateRemainPoint()).AddTo(this);
        //UpdateRemainPoint();
    }
    private void SaveData()
    {
        //DatabaseManager.Instance.SaveUserDataAsync(InGameDataManager.Instance.UserData);
        InGameDataManager.Instance.InGameData.TreasureData.TreasureUpgradeData = UpgradeData;
        InGameDataManager.Instance.InGameData.TreasureData.TreasureSlotData = SlotData;
        InGameDataManager.Instance.SaveData();
    }
    private void InitNewData()
    {
        for(int i = 0; i < TreasurePoolGlobalConfig.TreasureConfigs.Count; i++)
        {
            TreasureConfig treasureConfig = TreasurePoolGlobalConfig.TreasureConfigs[i];
            if (treasureConfig.Rarity == Rarity.Mythic)
            {
                UpgradeData.Data.Add(new EachTreasureUpgradeData(treasureConfig.Id, 0, 0));
            }
            else
            {
                UpgradeData.Data.Add(new EachTreasureUpgradeData(treasureConfig.Id, 1, 0));
            }
            
        }
        for(int i = 0; i< TreasurePoolGlobalConfig.NumSlot; i++)
        {
            SlotData.SlotData.Add(i, new(-1));
        }
        SaveData();
    }
    #endregion

    #region Get Set
    public TreasureConfig GetTreasureConfig(int id)
    {
        return TreasurePoolGlobalConfig.GetTreasureConfig(id);
    }
    #endregion

    #region Treasure Upgrade
    public EachTreasureUpgradeData GetEachTreasureUpgradeData(int id)
    {
        for (int i = 0; i < UpgradeData.Data.Count; i++)
        {
            if (UpgradeData.Data[i].Id == id)
            {
                return UpgradeData.Data[i];
            }
        }
        return new EachTreasureUpgradeData(id, 0, 0);
    }
    public int GetPieceNeededUpgradeTreasure(int id)
    {
        EachTreasureUpgradeData data = GetEachTreasureUpgradeData(id);
        if (data == null)
        {
            Debug.LogError($"TreasureId {id} is not found in TreasureUpgradeData");
            return 0;
        }
        TreasureConfig treasureConfig = TreasurePoolGlobalConfig.GetTreasureConfig(id);
        if (treasureConfig == null)
        {
            Debug.LogError($"TreasureId {id} is not found in TreasurePoolGlobalConfig");
            return 0;
        }
        TreasureUpgradeLevelConfig treasureLevelUpgradeConfig = TreasureUpgradeGlobalConfig.GetTreasureUpgradeLevelConfig(treasureConfig.Rarity, data.Level);
        return treasureLevelUpgradeConfig.PieceCost;
    }
    public bool IsUpgradeAbleTreasure(int id)
    {
        EachTreasureUpgradeData data = GetEachTreasureUpgradeData(id);
        if (data == null)
        {
            Debug.LogError($"TreasureId {id} is not found in TreasureUpgradeData");
            return false;
        }
        TreasureConfig treasureConfig = TreasurePoolGlobalConfig.GetTreasureConfig(id);
        if (treasureConfig == null)
        {
            Debug.LogError($"HeroId {id} is not found in HeroPoolGlobalConfig");
            return false;
        }
        TreasureUpgradeLevelConfig treasureLevelUpgradeConfig = TreasureUpgradeGlobalConfig.GetTreasureUpgradeLevelConfig(treasureConfig.Rarity, data.Level);
        if (treasureConfig.Rarity == Rarity.Mythic)
        {
            return IsHaveEnoughResourceUpgradeTreasureMythic(id);
        }
        return data.Piece >= treasureLevelUpgradeConfig.PieceCost 
            && PlayerResourceData.Instance.IsEnoughGameResource(treasureLevelUpgradeConfig.ResourceCost);
    }
    public bool IsHaveEnoughResourceUpgradeTreasureMythic(int id)
    {
        EachTreasureUpgradeData data = GetEachTreasureUpgradeData(id);
        if (data == null)
        {
            Debug.LogError($"TreasureId {id} is not found in TreasureUpgradeData");
            return false;
        }
        TreasureConfig treasureConfig = TreasurePoolGlobalConfig.GetTreasureConfig(id);
        if (treasureConfig == null)
        {
            Debug.LogError($"HeroId {id} is not found in HeroPoolGlobalConfig");
            return false;
        }
        TreasureUpgradeLevelConfig treasureLevelUpgradeConfig = TreasureUpgradeGlobalConfig.GetTreasureUpgradeLevelConfig(treasureConfig.Rarity, data.Level);
        if (treasureConfig.Rarity == Rarity.Mythic)
        {
            //TODO: Check mysthic resource
            //return InGameDataManager.Instance.UserData.UserResourceList.;
            return PlayerResourceData.Instance.IsEnoughGameResource(treasureLevelUpgradeConfig.ResourceCost)
                && PlayerResourceData.Instance.IsEnoughGameResource(GameResource.Type.MythicStone,treasureLevelUpgradeConfig.PieceCost);
                ;
        }
        else
        {
            return false;
        }
    }
    public void UpgradeTreasure(int id)
    {
        if (!IsUpgradeAbleTreasure(id)) return;
        EachTreasureUpgradeData data = GetEachTreasureUpgradeData(id);
        if (data == null)
        {
            Debug.LogError($"TreasureId {id} is not found in TreasureUpgradeData");
            return;
        }
        TreasureConfig treasureConfig = TreasurePoolGlobalConfig.GetTreasureConfig(id);
        if (treasureConfig == null)
        {
            Debug.LogError($"HeroId {id} is not found in HeroPoolGlobalConfig");
            return;
        }
        TreasureUpgradeLevelConfig treasureLevelUpgradeConfig = TreasureUpgradeGlobalConfig.GetTreasureUpgradeLevelConfig(treasureConfig.Rarity, data.Level);
        if (treasureConfig.Rarity == Rarity.Mythic)
        {
            //TODO: Consume mysthic resource
            PlayerResourceData.Instance.ConsumeGameResource(treasureLevelUpgradeConfig.ResourceCost);
            PlayerResourceData.Instance.ConsumeGameResource(GameResource.Type.MythicStone, treasureLevelUpgradeConfig.PieceCost);
        }
        else
        {
            data.AddLevel(1);
            data.RemovePiece(treasureLevelUpgradeConfig.PieceCost);
            PlayerResourceData.Instance.ConsumeGameResource(treasureLevelUpgradeConfig.ResourceCost);
            //TODO : Consume gold
        }
        SaveData();
    }
    public void AddTreasurePiece(int id, int piece)
    {
        TreasureConfig config = TreasurePoolGlobalConfig.GetTreasureConfig(id);
        switch (config.Rarity)
        {
            case Rarity.Common:
            case Rarity.Rare:
            case Rarity.Epic:
            case Rarity.Legendary:
                AddTreasurePieceNotMythic(id, piece);
                break;
            case Rarity.Mythic:
                AddTreasurePieceMythic(id, piece);
                break;
            default:
                Debug.LogError($"Rarity {config.Rarity} is not found in AddHeroPiece");
                break;
        }

    }
    private void AddTreasurePieceMythic(int id, int piece)
    {
        EachTreasureUpgradeData data = GetEachTreasureUpgradeData(id);
        if (data.Level < 1)
        {
            data.AddLevel(1);
        }
        else
        {
            PlayerResourceData.Instance.AddGameResource(GameResource.Type.MythicStone, TreasureGachaGlobalConfig.NumMythicStoneConvert);
        }
    }
    private void AddTreasurePieceNotMythic(int id, int piece)
    {
        EachTreasureUpgradeData data = GetEachTreasureUpgradeData(id);
        if (data.Level < 1)
        {
            data.AddLevel(1);
            piece -= 1;
        }
        if (piece > 0)
        {
            data.AddPiece(piece);
        }
    }
    public float GetTreasurePieceProcess(int id)
    {
        EachTreasureUpgradeData data = GetEachTreasureUpgradeData(id);
        TreasureConfig config = GetTreasureConfig(id);
        TreasureUpgradeLevelConfig upgradeLevelConfig = TreasureUpgradeGlobalConfig.GetTreasureUpgradeLevelConfig(config.Rarity, data.Level);
        if (config.Rarity == Rarity.Mythic)
        {
            //TODO : Get mythic resource
            return (float)PlayerResourceData.Instance.GetGameResource(GameResource.Type.MythicStone).Amount.ToFloat() / upgradeLevelConfig.PieceCost;
        }
        return (float)data.Piece / upgradeLevelConfig.PieceCost;
    }
    public BigNumber GetCostUpgradeTreasure(int id)
    {
        EachTreasureUpgradeData data = GetEachTreasureUpgradeData(id);
        TreasureConfig config = GetTreasureConfig(id);
        TreasureUpgradeLevelConfig upgradeLevelConfig = TreasureUpgradeGlobalConfig.GetTreasureUpgradeLevelConfig(config.Rarity, data.Level);
        //if (config.Rarity == Rarity.Mythic)
        //{
        //    //TODO : Get mythic resource
        //    return upgradeLevelConfig.ResourceCost;
        //}
        return upgradeLevelConfig.ResourceCost.Amount;
    }
    #endregion

    #region Treasure Summon
    public void SetCurrentStageSummon(int stage)
    {
        CurStageGacha.Value = stage;
    }
    public int GetNumAppearance()
    {
        return TreasureGachaGlobalConfig.NumAppearanceProbability.GetRandomItem();
    }
    public List<GachaTreasureConfig> GetListGachaTreasureConfig(int numAppearance)
    {
        List<GachaTreasureConfig> list = new List<GachaTreasureConfig>();
        for (int i = 0; i < numAppearance; i++)
        {
            list.Add(TreasureGachaGlobalConfig.GachaTreasureProbability.GetRandomItem());
        }
        return list;
    }
    public List<GachaTreasureReward> GetListGachaTreasureReward(List<GachaTreasureConfig> input, int timeSummon)
    {
        List<GachaTreasureReward> list = new List<GachaTreasureReward>();
        foreach (GachaTreasureConfig gahcahConfig in input)
        {
            if (gahcahConfig.IsAcquired())
            {
                list.Add(new GachaTreasureReward(gahcahConfig, true, timeSummon));
            }
            else
            {
                list.Add(new GachaTreasureReward(gahcahConfig, false, timeSummon));
            }
        }
        return list;
    }
    public TreasureConfig GetRandomTreasureConfigDataByRarity(Rarity rarity)
    {
        return TreasurePoolGlobalConfig.GetRandomTreasureConfigByRarity(rarity);
    }
    public void ClaimGachaReward(List<GachaTreasureReward> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].IsAcquired) continue;
            list[i].ClaimReward();
        }
    }
    public void UpgradeStageSummon(int value)
    {
        CurStageGacha.Value = value;
    }
    //private void ClaimSummonRewardHeroPiece(SummonHeroReward summonHeroReward)
    //{
    //    HeroConfigData heroConfigData = new();
    //    //switch(summonHeroReward.Config.Type)
    //    //{
    //    //    case SummonHeroType.HeroCommon:
    //    //        heroConfigData = GetRandomHeroConfigDataByRarityAndFamily(Combat.Rarity.Common);
    //    //        break;
    //    //    case SummonHeroType.HeroUncommon:
    //    //        heroConfigData = GetRandomHeroConfigDataByRarityAndFamily(Combat.Rarity.Common);
    //    //        break;
    //    //    case SummonHeroType.HeroRare:
    //    //        heroConfigData = GetRandomHeroConfigDataByRarityAndFamily(Combat.Rarity.Rare);
    //    //        break;
    //    //    case SummonHeroType.HeroEpic:
    //    //        heroConfigData = GetRandomHeroConfigDataByRarityAndFamily(Combat.Rarity.Epic);
    //    //        break;
    //    //    case SummonHeroType.HeroMythic:
    //    //        heroConfigData = GetRandomHeroConfigDataByRarityAndFamily(Combat.Rarity.Mythic);
    //    //        break;
    //    //    case SummonHeroType.HeroLegendary:
    //    //        heroConfigData = GetRandomHeroConfigDataByRarityAndFamily(Combat.Rarity.Mythic);
    //    //        break;
    //    //    default:
    //    //        Debug.LogError($"SummonHeroType {summonHeroReward.Config.Type} is not found in ClaimSummonRewardHeroPiece");
    //    //        break;
    //    //}
    //    EachHeroUpgradeData heroUpgradeData = GetEachHeroUpgradeData(heroConfigData.HeroId);
    //    AddHeroPiece(heroUpgradeData.HeroId, summonHeroReward.Config.RewardAmount);
    //}
    //private void ClaimSummonRewardResource(SummonHeroReward summonHeroReward)
    //{
    //    //TODO : Add resource
    //}

    #endregion
    #region Treasure Slot
    public int GetTreasureSlot(int slot)
    {
        return SlotData.SlotData[slot];
    }
    public BigNumber GetAvailableFuryPoint(BigNumber oldPoint)
    {
        BigNumber tmp = FuryPoint.Value + oldPoint;
        for(int i = 0;i< TreasurePoolGlobalConfig.NumSlot; i++)
        {
            if (SlotData.SlotData[i] != -1)
            {
                tmp -= GetTreasureConfig(SlotData.SlotData[i]).FuryPoint;
            }
        }
        return tmp;
    }
    public BigNumber GetAvailableArcanePoint(BigNumber oldPoint)
    {
        BigNumber tmp = ArcanePoint.Value + oldPoint;
        for (int i = 0; i < TreasurePoolGlobalConfig.NumSlot; i++)
        {
            if (SlotData.SlotData[i] != -1)
            {
                tmp -= GetTreasureConfig(SlotData.SlotData[i]).ArcanePoint;
            }
        }
        return tmp;
    }
    public BigNumber GetAvailableLuckPoint(BigNumber oldPoint)
    {
        BigNumber tmp = LuckPoint.Value + oldPoint;
        for (int i = 0; i < TreasurePoolGlobalConfig.NumSlot; i++)
        {
            if (SlotData.SlotData[i] != -1)
            {
                tmp -= GetTreasureConfig(SlotData.SlotData[i]).LuckPoint;
            }
        }
        return tmp;
    }
    public bool IsSlotEmpty(int slot)
    {
        return SlotData.SlotData[slot] == -1;
    }
    public bool IsPlaceableRing(int slot, int id)
    {
        TreasureConfig config = GetTreasureConfig(id);
        int oldPointArcane = 0;
        int oldPointFury = 0;
        int oldPointFocus = 0;
        if (SlotData.SlotData[slot] == -1)
        {
            oldPointArcane = 0;
            oldPointFury = 0;
            oldPointFocus = 0;
        }
        else
        {
            TreasureConfig oldConfig = GetTreasureConfig(SlotData.SlotData[slot]);
            oldPointArcane = oldConfig.ArcanePoint;
            oldPointFury = oldConfig.FuryPoint;
            oldPointFocus = oldConfig.LuckPoint;
        }
        if (config.ArcanePoint > GetAvailableArcanePoint(oldPointArcane) 
            || config.FuryPoint > GetAvailableFuryPoint(oldPointFury) 
            || config.LuckPoint > GetAvailableLuckPoint(oldPointFocus))
        {
            return false;
        }
        return true;
    }
    public void PlaceRingToSlot(int slot, int id)
    {
        SlotData.SlotData[slot].Value = id;
        UpdateRemainPoint();
        SaveData();
    }
    public void UnEquipTreasureBySlotId(int slot)
    {
        SlotData.SlotData[slot].Value = -1;
        UpdateRemainPoint();
        SaveData();
    }
    public void UnEquipTreasureByTreasureId(int id)
    {
        for (int i = 0; i < TreasurePoolGlobalConfig.NumSlot; i++)
        {
            if (SlotData.SlotData[i] == id)
            {
                SlotData.SlotData[i].Value = -1;
                break;
            }
        }
        UpdateRemainPoint();
        SaveData();
    }
    private void UpdateRemainPoint()
    {
        RemainArcanePoint.Value = GetAvailableArcanePoint(0);
        RemainFuryPoint.Value = GetAvailableFuryPoint(0);
        RemainLuckPoint.Value = GetAvailableLuckPoint(0);
    }
    public bool IsTreasureEuiped(int id)
    {
        for(int i = 0; i < TreasurePoolGlobalConfig.NumSlot; i++)
        {
            if(SlotData.SlotData[i] == id)
            {
                return true;
            }
        }
        return false;
    }
    public int GetSlotIdByTreasureId(int id)
    {
        for (int i = 0; i < TreasurePoolGlobalConfig.NumSlot; i++)
        {
            if (SlotData.SlotData[i] == id)
            {
                return i;
            }
        }
        return -1;
    }
    #endregion
}
