using Sirenix.OdinInspector;
using UnityEngine;

namespace TW.Utility.CustomScaleMethod
{
    public class PowerMethod : ScaleMethod
    {
        [field: SerializeField, HorizontalGroup] private float A {get; set;}
        [field: SerializeField, HorizontalGroup]private float B {get; set;}
        [field: SerializeField, HorizontalGroup]private float C {get; set;}
        [ShowInInspector] private string Description => $"result = {A} * value^{B} + {C}";
        public override float Evaluate(float value)
        {
            return A * Mathf.Pow(value, B) + C;
        }

        public override int Evaluate(int value)
        {
            return (int) (A * Mathf.Pow(value, B) + C);
        }
    }
}