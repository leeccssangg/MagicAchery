using System.Collections;
using System.Collections.Generic;
using TMPro;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.UI;

public class UIMapMonsterInfo : ACachedMonoBehaviour
{
    [field: SerializeField] public TextMeshProUGUI TxtMonsterName { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtPercentage { get; private set; }
    [field: SerializeField] public List<GameObject> ListRewardIcon { get; private set; }
    [field: SerializeField] public Image ImgIcon { get; private set; }

    public void Setup(int monsterId)
    {
        int mapId = InGameDataManager.Instance.InGameData.PlayerBattleData.JobId;
        int mapLevel = InGameDataManager.Instance.InGameData.PlayerBattleData.JobLevel;
        MapConfig mapConfig = MonsterGlobalConfig.Instance.GetMapConfig(mapId, mapLevel);
        Monster.Type type = (Monster.Type)monsterId;
        TxtMonsterName.text = type.ToString() + " Monster";
        switch (type)
        {
            case Monster.Type.Normal:
                TxtPercentage.text = $"{mapConfig.NormalMonsterSpawnRate}%";
                break;
            case Monster.Type.Rare:
                TxtPercentage.text = $"{mapConfig.RareMonsterSpawnRate}%";
                break;
            case Monster.Type.Epic:
                TxtPercentage.text = $"{mapConfig.EpicMonsterSpawnRate}%";
                break;
            case Monster.Type.Boss:
                TxtPercentage.text = $"{mapConfig.BossMonsterSpawnRate}%";
                break;
            default:
                TxtPercentage.text = "0%";
                break;
        }
    }
}
