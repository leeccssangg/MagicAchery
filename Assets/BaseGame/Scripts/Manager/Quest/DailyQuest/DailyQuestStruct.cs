public class DailyQuest_Login : DailyQuest
{
    public override void OnNotify(MissionTarget id, string info)
    {
        if (id != MissionTarget.LOGIN) return;
        int amount = int.Parse(info);
        OnCollect(amount);
    }
    public override MissionTarget GetMissionTarget()
    {
        return MissionTarget.LOGIN;
    }
    public override float GetProgress()
    {
        return (float)cl / (float)questConfig.targetAmount;
    }
}
public class DailyQuest_DestroyObstacle : DailyQuest
{
    public override void OnNotify(MissionTarget id, string info)
    {
        if (id != MissionTarget.DESTROY_OBSTACLE) return;
        int amount = int.Parse(info);
        OnCollect(amount);
    }
    public override MissionTarget GetMissionTarget()
    {
        return MissionTarget.DESTROY_OBSTACLE;
    }
    public override float GetProgress()
    {
        return (float)cl / (float)questConfig.targetAmount;
    }
}
public class DailyQuest_UpgradeATK : DailyQuest
{
    public override void OnNotify(MissionTarget id, string info)
    {
        if (id != MissionTarget.UPGRADE_ATK) return;
        int amount = int.Parse(info);
        OnCollect(amount);
    }
    public override MissionTarget GetMissionTarget()
    {
        return MissionTarget.UPGRADE_ATK;
    }
    public override float GetProgress()
    {
        return (float)cl / (float)questConfig.targetAmount;
    }
}
public class DailyQuest_UpgradeHP : DailyQuest
{

   public override void OnNotify(MissionTarget id, string info)
    {
        if (id != MissionTarget.UPGRADE_HP) return;
        int amount = int.Parse(info);
        OnCollect(amount);
    }
    public override MissionTarget GetMissionTarget()
    {
        return MissionTarget.UPGRADE_HP;
    }
    public override float GetProgress()
    {
        return (float)cl / (float)questConfig.targetAmount;
    }
}
public class DailyQuest_UpgradeMana : DailyQuest
{
    public override void OnNotify(MissionTarget id, string info)
    {
        if (id != MissionTarget.UPGRADE_MANA) return;
        int amount = int.Parse(info);
        OnCollect(amount);
    }
    public override MissionTarget GetMissionTarget()
    {
        return MissionTarget.UPGRADE_MANA;
    }
    public override float GetProgress()
    {
        return (float)cl / (float)questConfig.targetAmount;
    }
}
public class DailyQuest_UpgradeManaRegen : DailyQuest
{
    public override void OnNotify(MissionTarget id, string info)
    {
        if (id != MissionTarget.UPGRADE_MANA_REGEN) return;
        int amount = int.Parse(info);
        OnCollect(amount);
    }
    public override MissionTarget GetMissionTarget()
    {
        return MissionTarget.UPGRADE_MANA_REGEN;
    }
    public override float GetProgress()
    {
        return (float)cl / (float)questConfig.targetAmount;
    }
}
public class DailyQuest_HeadShot : DailyQuest
{
    public override void OnNotify(MissionTarget id, string info)
    {
        if (id != MissionTarget.LAND_HEADSHOT) return;
        int amount = int.Parse(info);
        OnCollect(amount);
    }
    public override MissionTarget GetMissionTarget()
    {
        return MissionTarget.LAND_HEADSHOT;
    }
    public override float GetProgress()
    {
        return (float)cl / (float)questConfig.targetAmount;
    }
}
public class DailyQuest_PassLevel : DailyQuest
{
    public override void OnNotify(MissionTarget id, string info)
    {
        if (id != MissionTarget.PASS_LEVEL) return;
        int amount = int.Parse(info);
        OnCollect(amount);
    }
    public override MissionTarget GetMissionTarget()
    {
        return MissionTarget.PASS_LEVEL;
    }
    public override float GetProgress()
    {
        return (float)cl / (float)questConfig.targetAmount;
    }
}
public class DailyQuest_WatchAds : DailyQuest
{
    public override void OnNotify(MissionTarget id, string info)
    {
        if (id != MissionTarget.WATCH_ADS) return;
        int amount = int.Parse(info);
        OnCollect(amount);
    }
    public override MissionTarget GetMissionTarget()
    {
        return MissionTarget.WATCH_ADS;
    }
    public override float GetProgress()
    {
        return (float)cl / (float)questConfig.targetAmount;
    }
}
public class DailyQuest_KillEnemy : DailyQuest
{
    public override void OnNotify(MissionTarget id, string info)
    {
        if (id != MissionTarget.KILL_ENEMY) return;
        int amount = int.Parse(info);
        OnCollect(amount);
    }
    public override MissionTarget GetMissionTarget()
    {
        return MissionTarget.KILL_ENEMY;
    }
    public override float GetProgress()
    {
        return (float)cl / (float)questConfig.targetAmount;
    }
}
public class DailyQuest_UpgradeWeapon : DailyQuest
{
    public override void OnNotify(MissionTarget id, string info)
    {
        if (id != MissionTarget.UPGRADE_WEAPON) return;
        int amount = int.Parse(info);
        OnCollect(amount);
    }
    public override MissionTarget GetMissionTarget()
    {
        return MissionTarget.UPGRADE_WEAPON;
    }
    public override float GetProgress()
    {
        return (float)cl / (float)questConfig.targetAmount;
    }
}
public class DailyQuest_UpgradeSkin : DailyQuest
{
    public override void OnNotify(MissionTarget id, string info)
    {
        if (id != MissionTarget.UPGRAGE_SKIN) return;
        int amount = int.Parse(info);
        OnCollect(amount);
    }
    public override MissionTarget GetMissionTarget()
    {
        return MissionTarget.UPGRAGE_SKIN;
    }
    public override float GetProgress()
    {
        return (float)cl / (float)questConfig.targetAmount;
    }
}


