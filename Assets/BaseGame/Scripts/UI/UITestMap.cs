using TMPro;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.UI;

public class UITestMap : ACachedMonoBehaviour
{
    [field: SerializeField] public TextMeshProUGUI TextMeshProUGUI {get; private set;}
    [field: SerializeField] public Button NextLevel {get; private set;}
    [field: SerializeField] public Button PreviousLevel {get; private set;}
    private ReactiveValue<int> Map {get; set;}
    private ReactiveValue<int> Level {get; set;}
    private void Start()
    {
        NextLevel.onClick.AddListener(OnClickNextLevel);
        PreviousLevel.onClick.AddListener(OnClickPreviousLevel);
        Map = PlayerBattleData.Instance.JobId;
        Level = PlayerBattleData.Instance.JobLevel;
        UpdateText();
    }
    private void UpdateText()
    {
        if (MonsterGlobalConfig.Instance.TryGetMapConfig(Map, Level, out var mapConfig))
        {
            TextMeshProUGUI.text = $"Map: {Map.Value} - Level: {Level.Value}\n" +
                                   $"Normal Monster: {mapConfig.NormalMonsterSpawnRate}%\n" +
                                   $"Rare Monster: {mapConfig.RareMonsterSpawnRate}%\n" +
                                   $"Epic Monster: {mapConfig.EpicMonsterSpawnRate}%\n" +
                                   $"Boss Monster: {mapConfig.BossMonsterSpawnRate}%";
        }
    }
    private void OnClickNextLevel()
    {
        if (!MonsterGlobalConfig.Instance.TryGetNextMapLevel(Map, Level, out int nextMap, out int nextLevel)) return;
        Map.Value = nextMap;
        Level.Value = nextLevel;
        UpdateText();
        InGameDataManager.Instance.SaveData();
    }
    private void OnClickPreviousLevel()
    {
        if (!MonsterGlobalConfig.Instance.TryGetPreviousMapLevel(Map, Level, out int previousMap, out int previousLevel)) return;
        Map.Value = previousMap;
        Level.Value = previousLevel;
        UpdateText();
        InGameDataManager.Instance.SaveData();
    }
}