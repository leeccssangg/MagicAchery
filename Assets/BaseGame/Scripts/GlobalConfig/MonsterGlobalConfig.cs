using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Utilities;
using TW.Utility.CustomType;
using TW.Utility.Extension;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "MonsterGlobalConfig", menuName = "GlobalConfigs/MonsterGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class MonsterGlobalConfig : GlobalConfig<MonsterGlobalConfig>
{
    [field: SerializeField] public MapConfig[] MapConfig { get; private set; }
    [field: SerializeField] public Monster[] MonsterPrefab { get; private set; }
    [field: SerializeField, TableList] public MonsterConfig[] NormalMonsterConfig { get; private set; }
    [field: SerializeField, TableList] public MonsterConfig[] RareMonsterConfig { get; private set; }
    [field: SerializeField, TableList] public MonsterConfig[] EpicMonsterConfig { get; private set; }
    [field: SerializeField, TableList] public MonsterConfig[] BossMonsterConfig { get; private set; }
    private List<int> MapLevelListCache { get; set; }

    private List<int> MapLevelList
    {
        get
        {
            if (MapLevelListCache == null || MapLevelListCache.Count == 0)
            {
                MapLevelListCache = MapConfig.Select(x => x.MapId * 100 + x.Level).OrderBy(x => x).ToList();
            }
            return MapLevelListCache;
        }
    }

    private Dictionary<int, MapConfig> MapConfigDictionaryCache { get; set; }

    private Dictionary<int, MapConfig> MapConfigDictionary =>
        MapConfigDictionaryCache ??= MapConfig.ToDictionary(x => x.MapId * 100 + x.Level);

    private Dictionary<int, MonsterConfig> NormalMonsterConfigDictionaryCache { get; set; }

    private Dictionary<int, MonsterConfig> NormalMonsterConfigDictionary => NormalMonsterConfigDictionaryCache ??=
        NormalMonsterConfig.ToDictionary(x => x.MapId * 100 + x.Level);

    private Dictionary<int, MonsterConfig> RareMonsterConfigDictionaryCache { get; set; }

    private Dictionary<int, MonsterConfig> RareMonsterConfigDictionary => RareMonsterConfigDictionaryCache ??=
        RareMonsterConfig.ToDictionary(x => x.MapId * 100 + x.Level);

    private Dictionary<int, MonsterConfig> EpicMonsterConfigDictionaryCache { get; set; }

    private Dictionary<int, MonsterConfig> EpicMonsterConfigDictionary => EpicMonsterConfigDictionaryCache ??=
        EpicMonsterConfig.ToDictionary(x => x.MapId * 100 + x.Level);

    private Dictionary<int, MonsterConfig> BossMonsterConfigDictionaryCache { get; set; }

    private Dictionary<int, MonsterConfig> BossMonsterConfigDictionary => BossMonsterConfigDictionaryCache ??=
        BossMonsterConfig.ToDictionary(x => x.MapId * 100 + x.Level);

    public Monster GetMonsterPrefab(int id)
    {
        return MonsterPrefab[id];
    }

    public bool TryGetMapConfig(int map, int level, out MapConfig mapConfig)
    {
        return MapConfigDictionary.TryGetValue(map * 100 + level, out mapConfig);
    }

    public bool TryGetNextMapLevel(int currentMap, int currentLevel, out int nextMap, out int nextLevel)
    {
        nextMap = -1;
        nextLevel = -1;
        for (int i = 0; i < MapLevelList.Count; i++)
        {
            if (MapLevelList[i] != currentMap * 100 + currentLevel) continue;
            if (i + 1 >= MapLevelList.Count) return false;
            nextMap = MapLevelList[i + 1] / 100;
            nextLevel = MapLevelList[i + 1] % 100;

            return true;
        }

        return false;
    }

    public bool TryGetPreviousMapLevel(int currentMap, int currentLevel, out int previousMap, out int previousLevel)
    {
        previousMap = -1;
        previousLevel = -1;
        for (int i = 0; i < MapLevelList.Count; i++)
        {
            if (MapLevelList[i] != currentMap * 100 + currentLevel) continue;
            if (i - 1 < 0) return false;
            previousMap = MapLevelList[i - 1] / 100;
            previousLevel = MapLevelList[i - 1] % 100;
            return true;
        }

        return false;
    }

    public MapConfig GetMapConfig(int map, int level)
    {
        return MapConfigDictionary[map * 100 + level];
    }

    public MonsterConfig GetMonsterConfig(Monster.Type monsterType, int map, int level)
    {
        switch (monsterType)
        {
            case Monster.Type.Normal:
                return NormalMonsterConfigDictionary[map * 100 + level];
            case Monster.Type.Rare:
                return RareMonsterConfigDictionary[map * 100 + level];
            case Monster.Type.Epic:
                return EpicMonsterConfigDictionary[map * 100 + level];
            case Monster.Type.Boss:
                return BossMonsterConfigDictionary[map * 100 + level];
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    [Button]
    private async UniTask UpdateData()
    {
        await UpdateMapData();
        await UpdateNormalMonsterData();
        await UpdateRareMonsterData();
        await UpdateEpicMonsterData();
        await UpdateBossMonsterData();
    }

    private async UniTask UpdateMapData()
    {
        try
        {
            EditorUtility.SetDirty(this);
            string sheetId = "1NyYPOmFsLkKvclxk4aSwqbJK52_TdbHzVTGHROg6QfQ";
            string csvData = await ABakingSheet.GetCsv(sheetId, "MapConfig");
            List<Dictionary<string, string>> dicData = ACsvReader.ReadDataFromString(csvData);
            MapConfig = dicData.Select(x =>
            {
                MapConfig mapConfig = new MapConfig
                {
                    MapId = int.Parse(x["MapId"]),
                    Level = int.Parse(x["Level"]),
                };

                mapConfig.MonsterSpawnProbability = new Probability<Monster.Type>(new List<Monster.Type>());
                mapConfig.MonsterSpawnProbability.m_ProbabilityValueList.Add(
                    new ProbabilityValue<Monster.Type>(Monster.Type.Normal, int.Parse(x["Normal"]),
                        mapConfig.MonsterSpawnProbability.UpdateTotalValue));
                mapConfig.MonsterSpawnProbability.m_ProbabilityValueList.Add(
                    new ProbabilityValue<Monster.Type>(Monster.Type.Rare, int.Parse(x["Rare"]),
                        mapConfig.MonsterSpawnProbability.UpdateTotalValue));
                mapConfig.MonsterSpawnProbability.m_ProbabilityValueList.Add(
                    new ProbabilityValue<Monster.Type>(Monster.Type.Epic, int.Parse(x["Epic"]),
                        mapConfig.MonsterSpawnProbability.UpdateTotalValue));
                mapConfig.MonsterSpawnProbability.m_ProbabilityValueList.Add(
                    new ProbabilityValue<Monster.Type>(Monster.Type.Boss, int.Parse(x["Boss"]),
                        mapConfig.MonsterSpawnProbability.UpdateTotalValue));
                return mapConfig;
            }).ToArray();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private async UniTask UpdateNormalMonsterData()
    {
        try
        {
            EditorUtility.SetDirty(this);

            string sheetId = "1NyYPOmFsLkKvclxk4aSwqbJK52_TdbHzVTGHROg6QfQ";
            string csvData = await ABakingSheet.GetCsv(sheetId, "NormalMonsterConfig");
            List<Dictionary<string, string>> dicData = ACsvReader.ReadDataFromString(csvData);
            NormalMonsterConfig = dicData.Select(x =>
            {
                MonsterConfig monsterConfig = new MonsterConfig
                {
                    MapId = int.Parse(x["MapId"]),
                    Level = int.Parse(x["Level"]),
                    PrefabId = int.Parse(x["PrefabId"]),
                    AttackDamage = new BigNumber(x["AttackDamage"]),
                    AttackSpeed = float.Parse(x["AttackSpeed"]),
                    MovementSpeed = float.Parse(x["MoveSpeed"]),
                    HitPoint = new BigNumber(x["HitPoint"]),
                    Experience = new BigNumber(x["Experience"]),
                    Amount = new IntRange()
                    {
                        m_Min = int.Parse(x["AmountMin"]),
                        m_Max = int.Parse(x["AmountMax"])
                    }
                };
                List<GameResource> resourceDrop = new List<GameResource>();
                GameResource coin = new GameResource(GameResource.Type.Coin, new BigNumber(x["ResourceDropCoin"]));
                GameResource unique =
                    new GameResource(GameResource.Type.NormalDrop, new BigNumber(x["ResourceDropUnique"]));
                if (coin.Amount > 0)
                {
                    resourceDrop.Add(coin);
                }

                if (unique.Amount > 0)
                {
                    resourceDrop.Add(unique);
                }

                monsterConfig.ResourceDrop = resourceDrop.ToArray();
                return monsterConfig;
            }).ToArray();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private async UniTask UpdateRareMonsterData()
    {
        try
        {
            EditorUtility.SetDirty(this);

            string sheetId = "1NyYPOmFsLkKvclxk4aSwqbJK52_TdbHzVTGHROg6QfQ";
            string csvData = await ABakingSheet.GetCsv(sheetId, "RareMonsterConfig");
            List<Dictionary<string, string>> dicData = ACsvReader.ReadDataFromString(csvData);
            RareMonsterConfig = dicData.Select(x =>
            {
                MonsterConfig monsterConfig = new MonsterConfig
                {
                    MapId = int.Parse(x["MapId"]),
                    Level = int.Parse(x["Level"]),
                    PrefabId = int.Parse(x["PrefabId"]),
                    AttackDamage = new BigNumber(x["AttackDamage"]),
                    AttackSpeed = float.Parse(x["AttackSpeed"]),
                    MovementSpeed = float.Parse(x["MoveSpeed"]),
                    HitPoint = new BigNumber(x["HitPoint"]),
                    Experience = new BigNumber(x["Experience"]),
                    Amount = new IntRange()
                    {
                        m_Min = int.Parse(x["AmountMin"]),
                        m_Max = int.Parse(x["AmountMax"])
                    }
                };
                List<GameResource> resourceDrop = new List<GameResource>();
                GameResource coin = new GameResource(GameResource.Type.Coin, new BigNumber(x["ResourceDropCoin"]));
                GameResource unique =
                    new GameResource(GameResource.Type.RareDrop, new BigNumber(x["ResourceDropUnique"]));
                if (coin.Amount > 0)
                {
                    resourceDrop.Add(coin);
                }

                if (unique.Amount > 0)
                {
                    resourceDrop.Add(unique);
                }

                monsterConfig.ResourceDrop = resourceDrop.ToArray();
                return monsterConfig;
            }).ToArray();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private async UniTask UpdateEpicMonsterData()
    {
        try
        {
            EditorUtility.SetDirty(this);

            string sheetId = "1NyYPOmFsLkKvclxk4aSwqbJK52_TdbHzVTGHROg6QfQ";
            string csvData = await ABakingSheet.GetCsv(sheetId, "EpicMonsterConfig");
            List<Dictionary<string, string>> dicData = ACsvReader.ReadDataFromString(csvData);
            EpicMonsterConfig = dicData.Select(x =>
            {
                MonsterConfig monsterConfig = new MonsterConfig
                {
                    MapId = int.Parse(x["MapId"]),
                    Level = int.Parse(x["Level"]),
                    PrefabId = int.Parse(x["PrefabId"]),
                    AttackDamage = new BigNumber(x["AttackDamage"]),
                    AttackSpeed = float.Parse(x["AttackSpeed"]),
                    MovementSpeed = float.Parse(x["MoveSpeed"]),
                    HitPoint = new BigNumber(x["HitPoint"]),
                    Experience = new BigNumber(x["Experience"]),
                    Amount = new IntRange()
                    {
                        m_Min = int.Parse(x["AmountMin"]),
                        m_Max = int.Parse(x["AmountMax"])
                    }
                };
                List<GameResource> resourceDrop = new List<GameResource>();
                GameResource coin = new GameResource(GameResource.Type.Coin, new BigNumber(x["ResourceDropCoin"]));
                GameResource unique =
                    new GameResource(GameResource.Type.EpicDrop, new BigNumber(x["ResourceDropUnique"]));
                if (coin.Amount > 0)
                {
                    resourceDrop.Add(coin);
                }

                if (unique.Amount > 0)
                {
                    resourceDrop.Add(unique);
                }

                monsterConfig.ResourceDrop = resourceDrop.ToArray();
                return monsterConfig;
            }).ToArray();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private async UniTask UpdateBossMonsterData()
    {
        
        EditorUtility.SetDirty(this);

        string sheetId = "1NyYPOmFsLkKvclxk4aSwqbJK52_TdbHzVTGHROg6QfQ";
        string csvData = await ABakingSheet.GetCsv(sheetId, "BossMonsterConfig");
        List<Dictionary<string, string>> dicData = ACsvReader.ReadDataFromString(csvData);
        BossMonsterConfig = dicData.Select(x =>
        {
            MonsterConfig monsterConfig = new MonsterConfig
            {
                MapId = int.Parse(x["MapId"]),
                Level = int.Parse(x["Level"]),
                PrefabId = int.Parse(x["PrefabId"]),
                AttackDamage = new BigNumber(x["AttackDamage"]),
                AttackSpeed = float.Parse(x["AttackSpeed"]),
                MovementSpeed = float.Parse(x["MoveSpeed"]),
                HitPoint = new BigNumber(x["HitPoint"]),
                Experience = new BigNumber(x["Experience"]),
                Amount = new IntRange()
                {
                    m_Min = int.Parse(x["AmountMin"]),
                    m_Max = int.Parse(x["AmountMax"])
                }
            };
            List<GameResource> resourceDrop = new List<GameResource>();
            GameResource coin = new GameResource(GameResource.Type.Coin, new BigNumber(x["ResourceDropCoin"]));
            GameResource unique =
                new GameResource(GameResource.Type.BossDrop, new BigNumber(x["ResourceDropUnique"]));
            if (coin.Amount > 0)
            {
                resourceDrop.Add(coin);
            }

            if (unique.Amount > 0)
            {
                resourceDrop.Add(unique);
            }

            monsterConfig.ResourceDrop = resourceDrop.ToArray();
            return monsterConfig;
        }).ToArray();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
}

[Serializable]
public class MonsterConfig
{
    [field: SerializeField, TableColumnWidth(75, false)]
    public int MapId { get; set; }

    [field: SerializeField, TableColumnWidth(75, false)]
    public int Level { get; set; }

    [field: SerializeField, TableColumnWidth(100, false), VerticalGroup("Prefab"), HideLabel]
    public int PrefabId { get; set; }


    [field: SerializeField, TableColumnWidth(150, false), LabelText("AD"), VerticalGroup("Stat")]
    public BigNumber AttackDamage { get; set; }

    [field: SerializeField, TableColumnWidth(150, false), LabelText("AS"), VerticalGroup("Stat")]
    public float AttackSpeed { get; set; }
    [field: SerializeField, TableColumnWidth(150, false), LabelText("MS"), VerticalGroup("Stat")]
    public float MovementSpeed { get; set; }

    [field: SerializeField, TableColumnWidth(150, false), LabelText("HP"), VerticalGroup("Stat")]
    public BigNumber HitPoint { get; set; }

    [field: SerializeField, TableColumnWidth(150, false), LabelText("EXP"), VerticalGroup("Stat")]
    public BigNumber Experience { get; set; }
    

    [field: SerializeField, TableColumnWidth(150, false)]
    public IntRange Amount { get; set; }

    [field: SerializeField, TableColumnWidth(200, false)]
    public GameResource[] ResourceDrop { get; set; }

    [ShowInInspector, TableColumnWidth(100, false), VerticalGroup("Prefab"), HideLabel, PreviewField(95)]
    public GameObject PrefabEditor
    {
        get
        {
            try
            {
                return MonsterGlobalConfig.Instance.MonsterPrefab[PrefabId].gameObject;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }

    public Monster GetMonsterPrefab()
    {
        return MonsterGlobalConfig.Instance.GetMonsterPrefab(PrefabId);
    }
}

[Serializable]
public class MapConfig
{
    [field: SerializeField] public int MapId { get; set; }
    [field: SerializeField] public int Level { get; set; }
    [field: SerializeField] public Probability<Monster.Type> MonsterSpawnProbability { get; set; }

    public float NormalMonsterSpawnRate
    {
        get
        {
            ProbabilityValue<Monster.Type> value =
                MonsterSpawnProbability.m_ProbabilityValueList.First(x => x.m_Object == Monster.Type.Normal);
            return value.m_Chance * 100f / MonsterSpawnProbability.Total;
        }
    }

    public float RareMonsterSpawnRate
    {
        get
        {
            ProbabilityValue<Monster.Type> value =
                MonsterSpawnProbability.m_ProbabilityValueList.First(x => x.m_Object == Monster.Type.Rare);
            return value.m_Chance * 100f / MonsterSpawnProbability.Total;
        }
    }

    public float EpicMonsterSpawnRate
    {
        get
        {
            ProbabilityValue<Monster.Type> value =
                MonsterSpawnProbability.m_ProbabilityValueList.First(x => x.m_Object == Monster.Type.Epic);
            return value.m_Chance * 100f / MonsterSpawnProbability.Total;
        }
    }

    public float BossMonsterSpawnRate
    {
        get
        {
            ProbabilityValue<Monster.Type> value =
                MonsterSpawnProbability.m_ProbabilityValueList.First(x => x.m_Object == Monster.Type.Boss);
            return value.m_Chance * 100f / MonsterSpawnProbability.Total;
        }
    }
}