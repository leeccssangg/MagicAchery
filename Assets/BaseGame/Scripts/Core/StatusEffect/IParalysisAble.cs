using TW.Utility.CustomType;

namespace Core.GameStatusEffect
{
    public interface IParalysisAble
    {
        public int ParalysisStack { get; set; }
        public void TakeParalysisDamage(BigNumber damage);
    }
}