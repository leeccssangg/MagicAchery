namespace TW.Utility.CustomScaleMethod
{
    [System.Serializable]
    public abstract class ScaleMethod
    {
        public abstract float Evaluate(float value);
        public abstract int Evaluate(int value);
    }
}