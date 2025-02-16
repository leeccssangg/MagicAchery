using Sirenix.OdinInspector;
using UnityEngine;

namespace TW.Utility.CustomScaleMethod
{
    public class ExponentialMethod : ScaleMethod
    {
        [field: SerializeField, HorizontalGroup] private float A {get; set;}
        [field: SerializeField, HorizontalGroup]private float B {get; set;}
        [ShowInInspector] private string Description => $"result = {A} * {B}^x";
        public override float Evaluate(float value)
        {
            return A * Mathf.Pow(B, value);
        }

        public override int Evaluate(int value)
        {
            return (int) (A * Mathf.Pow(B, value));
        }
        
    }
}