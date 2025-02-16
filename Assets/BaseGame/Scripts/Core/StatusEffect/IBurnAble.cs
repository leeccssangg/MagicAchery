using TW.Utility.CustomType;

namespace Core.GameStatusEffect
{
    public interface IBurnAble
    {
        public void TakeBurnDamage(BigNumber damage);
    }
}