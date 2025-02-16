using System;
using System.Collections.Generic;
using TW.Utility.CustomComponent;
using UnityEngine;

public class LoopBackground : ACachedMonoBehaviour
{
    [System.Serializable]
    public class LayerBackground
    {
        [field: SerializeField] private Transform[] BackgroundArray {get; set;}
        [field: SerializeField] private float Speed {get; set;}
        [field: SerializeField] private float LoopRange {get; set;}
        public void Evaluate(float deltaTime)
        {
            for (int i = 0; i < BackgroundArray.Length; i++)
            {
                BackgroundArray[i].localPosition += Vector3.left * (Speed * deltaTime);
                if (BackgroundArray[i].localPosition.x < -LoopRange)
                {
                    BackgroundArray[i].position += Vector3.right * (LoopRange * 2);
                }
            }
        }
    }
    private BattleManager BattleManagerCache { get; set; }
    private BattleManager BattleManager => BattleManagerCache ??= BattleManager.Instance;
    [field: SerializeField] private LayerBackground[] LayerBackgroundArray {get; set;}
    [field: SerializeField] private float MapMovementSpeed {get; set;}
    public Action<float> OnBackgroundUpdate { get; set; }
    public void UpdateBackground(float deltaTime)
    {
        for (int i = 0; i < LayerBackgroundArray.Length; i++)
        {
            LayerBackgroundArray[i].Evaluate(deltaTime);
        }
        BattleManager.OnUpdateMap?.Invoke(deltaTime * MapMovementSpeed);
    }
}
