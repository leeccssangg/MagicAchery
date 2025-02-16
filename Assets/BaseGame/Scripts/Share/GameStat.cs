using System;
using MemoryPack;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;

[MemoryPackable]
[Serializable]
public partial class GameStat
{
    public enum Type
    {
        Concentration = 0,
        Strength = 1,
        Dexterity = 2,
        Accuracy = 3,
        Vitality = 4,
        Intelligence = 5,
        Wisdom = 6,
        Luck = 7,
        Charisma = 8,
    }
    
    [MemoryPackIgnore]
    [field: HideLabel, HorizontalGroup(nameof(GameStat), 100)]
    [field: SerializeField] public Type StatType { get; private set; }
    
    [MemoryPackIgnore]
    [field: SerializeField, HideLabel, HorizontalGroup(nameof(GameStat))]
    public ReactiveValue<BigNumber> ReactiveLevel { get; private set; } = new();
    [MemoryPackIgnore]
    [field: SuffixLabel("@string.Format(\"/{0}\", CalculateExperienceToNextLevel().ToStringUI())", true)]
    [field: SerializeField, HideLabel, HorizontalGroup(nameof(GameStat))]
    public ReactiveValue<BigNumber> ReactiveExperience { get; private set; } = new();

    [MemoryPackIgnore]
    public BigNumber Level
    {
        get => ReactiveLevel.Value;
        set => ReactiveLevel.Value = value;
    }
    
    [MemoryPackIgnore]
    public BigNumber Experience
    {
        get => ReactiveExperience.Value;
        set => ReactiveExperience.Value = value;
    }

    
    public GameStatData GameStatData { get; set; } = new();

    [MemoryPackConstructor]
    public GameStat()
    {
        
    }
    
    [MemoryPackOnSerializing]
    private void OnSerializing()
    {
        GameStatData.StatType = StatType;
        GameStatData.Lc = Level.coefficient;
        GameStatData.Le = Level.exponent;
        GameStatData.Ec = Experience.coefficient;
        GameStatData.Ee = Experience.exponent;
    }
    
    [MemoryPackOnDeserialized]
    private void OnDeserialized()
    {
        StatType = GameStatData.StatType;
        Level = new BigNumber(GameStatData.Lc, GameStatData.Le);
        Experience = new BigNumber(GameStatData.Ec, GameStatData.Ee);
    }
    
    public GameStat(Type statType, BigNumber level, BigNumber experience)
    {
        StatType = statType;
        Level = level;
        Experience = experience;
    }
    
    public GameStatData ToGameResourceData()
    {
        return new GameStatData()
        {
            StatType = StatType,
            Lc = Level.coefficient,
            Le = Level.exponent,
            Ec = Experience.coefficient,
            Ee = Experience.exponent,
        };
    }
    public GameStat FromGameResourceData(GameStatData gameStatData)
    {
        StatType = gameStatData.StatType;
        Level = new BigNumber(gameStatData.Lc, gameStatData.Le);
        Experience = new BigNumber(gameStatData.Ec, gameStatData.Ee);
        return this;
    }
    public void AddExperience(BigNumber value)
    {
        Experience += value;
        UpdateLevel();
    }
    public void AddLevel(BigNumber value)
    {
        Level += value;
    }
    public void ConsumeExperience(BigNumber value)
    {
        Experience -= value;
    }
    public void ConsumeLevel(BigNumber value)
    {
        Level -= value;
    }

    public void SetExperience(BigNumber value)
    {
        Experience = value;
    }

    public void SetLevel(BigNumber value)
    {
        Level = value;
    }
    private void UpdateLevel()
    {
        BigNumber experienceToNextLevel = CalculateExperienceToNextLevel();
        if (Experience < experienceToNextLevel) return;
        Experience -= experienceToNextLevel;
        Level += 1;
        UpdateLevel();
    }
    private BigNumber CalculateExperienceToNextLevel()
    {
        return CalculateExperienceToLevel(Level + 1);
    }
    public static BigNumber CalculateExperienceToLevel(BigNumber targetLevel)
    {
        return 90 + targetLevel.Pow(1.8f) * 10;
    }
}   

[MemoryPackable]
[Serializable]
public partial class GameStatData
{
    [field: SerializeField] public GameStat.Type StatType { get; set; }
    [field: SerializeField] public double Lc { get; set; }
    [field: SerializeField] public int Le { get; set; }
    [field: SerializeField] public double Ec { get; set; }
    [field: SerializeField] public int Ee { get; set; }
}