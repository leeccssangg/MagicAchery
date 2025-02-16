using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TW.UGUI.Utility;
using UnityEngine;

namespace TW.UGUI.Shared
{
    public class GroupTransitionAnimationBehaviour : TransitionAnimationBehaviour
    {
        [field: TableList]
        [field: SerializeReference] public List<IUnitTransitionAnimation> UnitTransitionAnimation { get; private set; } = new();
        
        
        public override void Setup()
        {
            foreach (IUnitTransitionAnimation unit in UnitTransitionAnimation)
            {
                unit.Initialize();
                unit.Setup();
            }
        }

        public override async UniTask PlayAsync(IProgress<float> progress = null)
        {
            
        }

        public override void Play(IProgress<float> progress = null)
        {
            
        }

        public override void Stop()
        {
            
        }

        public override float TotalDuration { get; }
        
        private void ForceSetup()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            foreach (IUnitTransitionAnimation unit in UnitTransitionAnimation)
            {
                unit.Setup();
            }
#endif
        }
    }
}
