using System.Collections.Generic;
using System.Linq;
using MemoryPack;
using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class PlayerResourceData
{
    private static PlayerResourceData InstanceCache { get; set; }
    public static PlayerResourceData Instance => InstanceCache ??= InGameDataManager.Instance.InGameData.PlayerResourceData;
    [field: SerializeField] public List<GameResource> GameResourceList { get; set; }

    [MemoryPackConstructor]
    public PlayerResourceData()
    {
        // TODO: Create default player resource value
        GameResourceList = new List<GameResource>()
        {
            new GameResource(GameResource.Type.Coin, 0),
            new GameResource(GameResource.Type.Gem, 0),
        };
    }

    public GameResource GetGameResource(GameResource.Type resourceType)
    {
        GameResource resource = null;
        for (int i = 0; i < GameResourceList.Count; i++)
        {
            if (GameResourceList[i].ResourceType == resourceType)
            {
                resource = GameResourceList[i];
                break;
            }
        }
        if (resource == null)
        {
            resource = new GameResource(resourceType, 0);
            GameResourceList.Add(resource);
        }
        return resource;
    }
    public void AddGameResource(GameResource.Type resourceType, BigNumber value)
    {
        GetGameResource(resourceType).Add(value);
    }
    public void AddGameResource(GameResource gameResource)
    {
        AddGameResource(gameResource.ResourceType, gameResource.Amount);
    }
    public void ConsumeGameResource(GameResource.Type resourceType, BigNumber value)
    {
        GetGameResource(resourceType).Consume(value);
    }
    public void ConsumeGameResource(GameResource gameResource)
    {
        ConsumeGameResource(gameResource.ResourceType, gameResource.Amount);
    }
    public bool IsEnoughGameResource(GameResource gameResource)
    {
        return IsEnoughGameResource(gameResource.ResourceType, gameResource.Amount);
    }
    public bool IsEnoughGameResource(GameResource.Type resourceType, BigNumber value)
    {
        return GetGameResource(resourceType).IsEnough(value);
    }
    public void ClaimListResources(List<GameResource> gameResources, int multiplier = 1)
    {
        for (int i = 0; i < gameResources.Count; i++)
        {
            AddGameResource(gameResources[i].ResourceType, gameResources[i].Amount * multiplier);
        }
    }
}

public partial class InGameData
{
    [MemoryPackOrder(2)] 
    [field: SerializeField, PropertyOrder(2)] public PlayerResourceData PlayerResourceData { get; set; } = new();
}