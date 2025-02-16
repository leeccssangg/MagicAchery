using TW.Reactive.CustomComponent;
using UnityEngine;

public class Achivement<T> : Observer<T>
{
    public int id;
    public int type;
    public int collected;
    public int targetAmount;
    //public int icd;
    public int missionTarget;
    //public int point;
    public int level;
    public int maxLevel;
    public int offset;

    public virtual void Init() { }
    public virtual void Load() { }
    public override void OnNotify(T id, string info)
    {
    }
    public virtual void OnCollect(int amount)
    {
        if (collected < targetAmount)
        {
            collected += amount;
            Mathf.Clamp(collected, 0, targetAmount);
        }
    }
    public virtual void OnClaim()
    {
        //icd = 1;
        level++;
        UpdateTargetAmount();
        collected = 0;
    }
    public virtual void UpdateTargetAmount()
    {
        targetAmount = targetAmount + level * offset;
    }
    public virtual string GetDescription()
    {
        return "";
    }
    //public virtual int GetPoint()
    //{
    //    return point;
    //}
    public virtual float GetProgress()
    {
        float c = (float)collected / (float)targetAmount;
        if (c >= 1) c = 1;
        return c;
    }
    public virtual string GetProgressString()
    {
        string s = "" + collected + "/" + targetAmount;
        return s;
    }
    public int GetLevel()
    {
        return level;
    }
    public bool IsMaxLevel()
    {
        return level >= maxLevel;
    }
    public virtual bool IsCompleted()
    {
        return collected >= targetAmount;
    }
    public virtual int GetAchivementType()
    {
        return this.type;
    }
    public virtual string ToJsonString()
    {
        string json = JsonUtility.ToJson(this);
        return json;
    }
    public static Achivement<T> FromJson(string s)
    {
        return JsonUtility.FromJson<Achivement<T>>(s);
    }
}
