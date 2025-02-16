using TW.Utility.CustomType;

namespace Core.AttributeHeal
{
    public interface IHeal
    {
        void TakeHeal(BigNumber heal);
        bool IsShielded();
        bool IsInCombat();
    }
}