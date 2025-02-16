using Sirenix.OdinInspector;
using UnityEngine;

namespace TW.Utility.CustomScaleMethod
{
    public class QuadraticMethod : ScaleMethod
    {
        [field: SerializeField, HorizontalGroup] private float A {get; set;}
        [field: SerializeField, HorizontalGroup]private float B {get; set;}
        [field: SerializeField, HorizontalGroup]private float C {get; set;}
        [ShowInInspector] private string Description => $"result = {A} * value^2 + {B} * value + {C}";
        public override float Evaluate(float value)
        {
            return A * value * value + B * value + C;
        }

        public override int Evaluate(int value)
        {
            return (int) (A * value * value + B * value + C);
        }
    }
}