using System;
using System.Collections.Generic;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using R3;
using TW.ACacheEverything;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using TW.Utility.Extension;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class BattleManager : Singleton<BattleManager>
{
    private InGameDataManager InGameDataManagerCache { get; set; }
    private InGameDataManager InGameDataManager => InGameDataManagerCache ??= InGameDataManager.Instance;
    private PlayerStatData PlayerStatDataCache { get; set; }
    private PlayerStatData PlayerStatData => PlayerStatDataCache ??= PlayerStatData.Instance;
    private PlayerBattleData PlayerBattleDataCache { get; set; }
    private PlayerBattleData PlayerBattleData => PlayerBattleDataCache ??= PlayerBattleData.Instance;
    private TalentTreeManager TalentTreeManagerCache { get; set; }
    private TalentTreeManager TalentTreeManager => TalentTreeManagerCache ??= TalentTreeManager.Instance;
    private FactoryManager FactoryManagerCache { get; set; }
    private FactoryManager FactoryManager => FactoryManagerCache ??= FactoryManager.Instance;
    private StateMachine StateMachine { get; set; }
    [field: SerializeField] public ReactiveValue<GameStat.Type> CurrentTrainingStatType { get; private set; }
    [field: SerializeField] public ReactiveValue<int> TrainingMap { get; private set; }
    [field: SerializeField] public ReactiveValue<int> TrainingLevel { get; private set; }
    [field: SerializeField] public Hero Hero {get; private set;}
    [field: SerializeField] public LoopBackground LoopBackground { get; private set; }
    [field: SerializeField] public Transform SpawnPosition {get; private set;}
    [field: SerializeField] public Transform WaitPosition {get; private set;}
    [field: SerializeField] public List<Monster> MonsterList {get; private set;} = new List<Monster>();
    [field: SerializeField] public float FindingMonsterBaseTime {get; private set;}
    [field: SerializeField] public float StartBattleDistance {get; private set;}
    public Action<float> OnUpdateMap { get; set; } 
    private float CurrentFindingMonsterTime { get; set; }
    private bool IsSpawnMonster { get; set; }
    private void Start()
    {
        InitStateMachine();
        StartBattle();
        
        CurrentTrainingStatType = PlayerBattleData.CurrentTrainingStatType;
        TrainingMap = PlayerBattleData.JobId;
        TrainingLevel = PlayerBattleData.JobLevel;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        StateMachine.Stop();
    }

    private void InitStateMachine()
    {
        StateMachine = new StateMachine();
        StateMachine.RegisterState(SleepState);
        StateMachine.Run();
    }
    
    private void StartBattle()
    {
        StateMachine.RequestTransition(IdleState);
    }

    public int GetMonsterNonAlloc(Monster[] monsterArray)
    {
        int count = 0;
        for (int i = 0; i < MonsterList.Count; i++)
        {
            if (MonsterList[i].IsDead) continue;
            if (MonsterList[i].IsFutureDead) continue;
            monsterArray[count] = MonsterList[i];
            count++;
        }
        return count;
    }
    public bool TryGetClosestMonster(out Monster monster)
    {
        monster = null;
        Monster[] monsterArray = new Monster[10];
        int monsterCount = GetMonsterNonAlloc(monsterArray);
        float minDistance = float.MaxValue;
        for (int i = 0; i < monsterCount; i++)
        {
            float distance = Vector3.Distance(Hero.Transform.position, monsterArray[i].Transform.position);
            if (!(distance < minDistance)) continue;
            minDistance = distance;
            monster = monsterArray[i];
        }
        return monster != null;
    }
    public void GainExperience(BigNumber experience, Vector3 position)
    {
        BigNumber bonus = GetCurrentTrainingStatBonus();
        experience = experience * (1 + PlayerStatData[GameStat.Type.Concentration].Level * 5 / 100f + bonus / 100f);
        FactoryManager.SpawnExperienceText(experience, position);
        PlayerStatData[CurrentTrainingStatType].AddExperience(experience);
        InGameDataManager.SaveData();
    }
    public void AddMonster(Monster monster)
    {
        MonsterList.Add(monster);
    }
    public void RemoveMonster(Monster monster)
    {
        MonsterList.Remove(monster);
    }

    private void SpawnMonster()
    {
        MapConfig mapConfig = MonsterGlobalConfig.Instance.GetMapConfig(TrainingMap, TrainingLevel);
        Monster.Type monsterType = mapConfig.MonsterSpawnProbability.GetRandomItem();
        MonsterConfig monsterConfig = MonsterGlobalConfig.Instance.GetMonsterConfig(monsterType, TrainingMap, TrainingLevel);
        int amount = monsterConfig.Amount.GetRandomValue();
        int bonusAmount = GetBonusMonsterAmount(monsterType);

        for (int i = 0; i < amount + bonusAmount; i++)
        {
            Monster monsterPrefab = monsterConfig.GetMonsterPrefab();
            Vector3 spawnPosition = SpawnPosition.position + Random.insideUnitSphere * 3;
            spawnPosition.z = 0;
            Monster monster = monsterPrefab
                .Spawn(spawnPosition, Quaternion.identity, Transform)
                .InitStat(monsterConfig);
        }
    }
    public int GetBonusMonsterAmount(Monster.Type monsterType)
    {
        switch (monsterType)
        {
            case Monster.Type.Normal:
                return TalentTreeManager.GetTalentStat(TalentStat.Type.NormalEnemyAmount).Amount.ToInt();
            case Monster.Type.Rare:
                return TalentTreeManager.GetTalentStat(TalentStat.Type.RareEnemyAmount).Amount.ToInt();
            case Monster.Type.Epic:
                return TalentTreeManager.GetTalentStat(TalentStat.Type.EpicEnemyAmount).Amount.ToInt();
        }
        return 0;
    }
    public BigNumber GetCurrentTrainingStatBonus()
    {
        return CurrentTrainingStatType.Value switch
        {
            GameStat.Type.Concentration => TalentTreeManager.GetTalentStat(TalentStat.Type.ConcentrationExpBonus).Amount,
            GameStat.Type.Strength => TalentTreeManager.GetTalentStat(TalentStat.Type.StrengthExpBonus).Amount,
            GameStat.Type.Dexterity => TalentTreeManager.GetTalentStat(TalentStat.Type.DexterityExpBonus).Amount,
            GameStat.Type.Accuracy => TalentTreeManager.GetTalentStat(TalentStat.Type.AccuracyExpBonus).Amount,
            GameStat.Type.Vitality => TalentTreeManager.GetTalentStat(TalentStat.Type.VitalityExpBonus).Amount,
            GameStat.Type.Intelligence => TalentTreeManager.GetTalentStat(TalentStat.Type.IntelligenceExpBonus).Amount,
            _ => BigNumber.ZERO
        };
    }

    public void UpdateBackground(float deltaTime)
    {
        LoopBackground.UpdateBackground(deltaTime);
    }
}