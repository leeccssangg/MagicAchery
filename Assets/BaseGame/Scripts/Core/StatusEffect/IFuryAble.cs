using R3;

namespace Core.GameStatusEffect
{
    public interface IFuryAble
    {
        public SerializableReactiveProperty<float> FuryAttackSpeedBuff { get; set; }
    }
}