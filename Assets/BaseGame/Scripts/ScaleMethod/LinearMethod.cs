using Sirenix.OdinInspector;
using UnityEngine;

namespace TW.Utility.CustomScaleMethod
{
    [System.Serializable]
    public class LinearMethod : ScaleMethod
    {
        [field: SerializeField, HorizontalGroup] private float A {get; set;}
        [field: SerializeField, HorizontalGroup]private float B {get; set;}
        [ShowInInspector] private string Description => $"result = {A} * value + {B}";
        public override float Evaluate(float value)
        {
            return A * value + B;
        }

        public override int Evaluate(int value)
        {
            return (int) (A * value + B);
        }
    }
}