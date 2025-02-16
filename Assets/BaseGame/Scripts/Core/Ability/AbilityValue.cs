using UnityEngine;

namespace Core.HeroAbility
{
	[System.Serializable]
    public class AbilityValue<T>
    {
	    [field: SerializeField] public T Level0 {get; private set;}
	    [field: SerializeField] public T Level6 {get; private set;}
	    [field: SerializeField] public T Level12 {get; private set;}

	    public T GetValue(int level)
	    {
		    return level switch
		    {
			    >= 12 => Level12,
			    >= 6 => Level6,
			    _ => Level0
		    };
	    }
    }
}