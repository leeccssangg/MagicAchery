using UnityEngine;
using Sirenix.Utilities;
using TW.Utility.CustomType;
using Sirenix.OdinInspector;
using Lofelt.NiceVibrations;

public enum GachaTreasureType
{
    None,
    TreasureCommon,
    TreasureRare,
    TreasureEpic,
    TreasureLegendary,
    TreasureMythic,
    Gold,
    Gem,
    MythicStone,
}

[CreateAssetMenu(fileName = "TreasureGachaGlobalConfig", menuName = "GlobalConfigs/TreasureGachaGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class TreasureGachaGlobalConfig : GlobalConfig<TreasureGachaGlobalConfig>
{
    [field: SerializeField] public Probability<int> NumAppearanceProbability { get; private set; } = new(new());
    [field: SerializeField] public Probability<GachaTreasureConfig> GachaTreasureProbability { get; private set; } = new(new());
    [field: SerializeField] public int StartStage { get; private set; }
    [field: SerializeField] public int NumMythicStoneConvert { get; private set; }

}
[System.Serializable]
public class GachaTreasureConfig
{
    [field: SerializeField] public GachaTreasureType Type { get; private set; }
    [field: SerializeField] public int RewardAmount { get; private set; }
    [MaxValue(100)]
    [field: SerializeField] public int AcquisitionRate { get; private set; }

    public bool IsAcquired()
    {
        return Random.Range(0, 100) <= AcquisitionRate;
    }
}
[System.Serializable]
public class GachaTreasureReward
{
    [field: SerializeField] public GachaTreasureConfig Config { get; private set; }
    [field: SerializeField] public bool IsAcquired { get; private set; }
    [field: SerializeField] public int NumGacha { get; private set; }
    [field: SerializeField] public TreasureConfig RewardTreasureConfig { get; private set; }
    [field: SerializeField] public GameResource RewardResource { get; private set; }

    public GachaTreasureReward(GachaTreasureConfig config, bool isAccquired, int numGacha)
    {
        Config = config;
        NumGacha = numGacha;
        IsAcquired = isAccquired;
        GetRandomTreasureConfig();
        GetResourceReward();
    }
    private void GetRandomTreasureConfig()
    {
        if (Config.Type == GachaTreasureType.None
            || Config.Type == GachaTreasureType.Gold
            || Config.Type == GachaTreasureType.Gem
            || Config.Type == GachaTreasureType.MythicStone)
            return;
        switch (Config.Type)
        {
            case GachaTreasureType.TreasureCommon:
                RewardTreasureConfig = TreasurePoolGlobalConfig.Instance.GetRandomTreasureConfigByRarity(Rarity.Common);
                break;
            case GachaTreasureType.TreasureRare:
                RewardTreasureConfig = TreasurePoolGlobalConfig.Instance.GetRandomTreasureConfigByRarity(Rarity.Rare);
                break;
            case GachaTreasureType.TreasureEpic:
                RewardTreasureConfig = TreasurePoolGlobalConfig.Instance.GetRandomTreasureConfigByRarity(Rarity.Epic);
                break;
            case GachaTreasureType.TreasureLegendary:
                RewardTreasureConfig = TreasurePoolGlobalConfig.Instance.GetRandomTreasureConfigByRarity(Rarity.Legendary);
                break;
            case GachaTreasureType.TreasureMythic:
                RewardTreasureConfig = TreasurePoolGlobalConfig.Instance.GetRandomTreasureConfigByRarity(Rarity.Mythic);
                break;
            default:
                break;
        }
    }
    private void GetResourceReward()
    {
        if (Config.Type != GachaTreasureType.Gold
           && Config.Type != GachaTreasureType.Gem
           && Config.Type != GachaTreasureType.MythicStone)
            return;
        switch (Config.Type)
        {
            case GachaTreasureType.Gold:
                RewardResource = new(GameResource.Type.Coin, new BigNumber(Config.RewardAmount));
                break;
            case GachaTreasureType.Gem:
                RewardResource = new(GameResource.Type.Gem, new BigNumber(Config.RewardAmount));
                break;
            case GachaTreasureType.MythicStone:
                RewardResource = new(GameResource.Type.MythicStone, new BigNumber(Config.RewardAmount));
                break;
            default:
                break;
        }
    }
    public void ClaimReward()
    {
        if (Config.Type != GachaTreasureType.None
            || Config.Type != GachaTreasureType.Gold
            || Config.Type != GachaTreasureType.Gem
            || Config.Type != GachaTreasureType.MythicStone)
        {
            EachTreasureUpgradeData data = TreasureManager.Instance.GetEachTreasureUpgradeData(RewardTreasureConfig.Id);
            TreasureManager.Instance.AddTreasurePiece(data.Id, Config.RewardAmount * NumGacha);
        }
        else
        {
            PlayerResourceData.Instance.AddGameResource(RewardResource.ResourceType, RewardResource.Amount*NumGacha);
        }
    }
}
    