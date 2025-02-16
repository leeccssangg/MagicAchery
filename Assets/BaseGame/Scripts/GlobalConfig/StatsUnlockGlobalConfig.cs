using UnityEngine;
using Sirenix.Utilities;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StatsUnlockGlobalConfig", menuName = "GlobalConfigs/StatsUnlockGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class StatsUnlockGlobalConfig : GlobalConfig<StatsUnlockGlobalConfig>
{
    public List<StatUnlock> statUnlocks = new();

    public StatUnlock GetStatUnlock(int id)
    {
        for (int i = 0; i < statUnlocks.Count; i++)
        {
            if (statUnlocks[i].id == id)
            {
                return statUnlocks[i];
            }
        }
        return null;
    }
    public bool IsStatUnlocked(GameStat.Type type, int idUnlocked)
    {
        for(int i = 0; i < statUnlocks.Count; i++)
        {
            if(statUnlocks[i].type == type && statUnlocks[i].id <= idUnlocked)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsStatUnlockNext(GameStat.Type type, int idUnlocked)
    {
        for(int i = 0; i < statUnlocks.Count; i++)
        {
            if(statUnlocks[i].type == type && statUnlocks[i].id == idUnlocked + 1)
            {
                return true;
            }
        }
        return false;
    }
}
[System.Serializable]
public class StatUnlock
{
    public int id;
    public GameStat.Type type;
    public GameResource cost;
}