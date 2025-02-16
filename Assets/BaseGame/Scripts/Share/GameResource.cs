using System;
using MemoryPack;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;

[MemoryPackable]
[Serializable] 
public partial class GameResource
{
    public enum Type
    {
        Coin = 0,
        Gem = 1,
        
        
        NormalDrop = 100,
        RareDrop = 101,
        EpicDrop = 102,
        BossDrop = 103,

        MythicStone = 200,
    }
    [MemoryPackIgnore]
    [field: HideLabel, HorizontalGroup(nameof(GameResource), 100)]
    [field: SerializeField] public Type ResourceType { get; private set; }
    
    [MemoryPackIgnore]
    [field: SerializeField, HideLabel, HorizontalGroup(nameof(GameResource))]
    public ReactiveValue<BigNumber> ReactiveAmount { get; private set; } = new();
    
    [MemoryPackIgnore]
    public BigNumber Amount
    {
        get => ReactiveAmount.Value;
        set => ReactiveAmount.Value = value;
    }
    
    
    public GameResourceData GameResourceData { get; set; } = new();

    [MemoryPackConstructor]
    public GameResource()
    {
        
    }
    
    [MemoryPackOnSerializing]
    private void OnSerializing()
    {
        GameResourceData.ResourceType = ResourceType;
        GameResourceData.C = Amount.coefficient;
        GameResourceData.E = Amount.exponent;
    }
    
    [MemoryPackOnDeserialized]
    private void OnDeserialized()
    {
        ResourceType = GameResourceData.ResourceType;
        Amount = new BigNumber(GameResourceData.C, GameResourceData.E);
    }
    
    public GameResource(Type resourceType, BigNumber amount)
    {
        ResourceType = resourceType;
        Amount = amount;
    }
    
    public GameResourceData ToGameResourceData()
    {
        return new GameResourceData()
        {
            ResourceType = ResourceType,
            C = Amount.coefficient,
            E = Amount.exponent,
        };
    }
    public GameResource FromGameResourceData(GameResourceData gameResourceData)
    {
        ResourceType = gameResourceData.ResourceType;
        Amount = new BigNumber(gameResourceData.C, gameResourceData.E);
        return this;
    }

    public void Add(BigNumber value)
    {
        Amount += value;
    }
    public void Consume(BigNumber value)
    {
        Amount -= value;
    }
    public bool IsEnough(BigNumber value, float threshold = 0.001f)
    {
        return value <= Amount + threshold;
    }

}

[MemoryPackable]
[Serializable]
public partial class GameResourceData
{
    [field: SerializeField] public GameResource.Type ResourceType { get; set; }
    [field: SerializeField] public double C { get; set; }
    [field: SerializeField] public int E { get; set; }
}