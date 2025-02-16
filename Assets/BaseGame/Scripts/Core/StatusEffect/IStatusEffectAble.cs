namespace Core.GameStatusEffect
{
    public interface IStatusEffectAble
    {
        public void AddStatusEffect(StatusEffect statusEffect);
        public void RemoveStatusEffect(StatusEffect statusEffect);

    }

    public static class StatusEffectExtension
    {
        public static bool As<T>(this IStatusEffectAble statusEffectAble, out T owner)
        {
            owner = (T)statusEffectAble;
            return owner != null;
        }

    }
}