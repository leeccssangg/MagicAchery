namespace Core.HeroAbility.AbilityTrigger
{
    public interface IAbilitySingleMonsterTarget
    {
        BattleManager BattleManager { get; }
        Monster TargetMonster { get; set; }
    }
    
    public static class AbilitySingleMonsterTargetExtension
    {
        public static bool TryFindAnyMonsterTarget(this IAbilitySingleMonsterTarget ability)
        {
            if (!ability.BattleManager.TryGetClosestMonster(out Monster monster)) return false;
            ability.TargetMonster = monster;
            return true;
        }
    }
}