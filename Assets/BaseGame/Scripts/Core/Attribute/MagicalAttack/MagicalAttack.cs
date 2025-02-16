using R3;
using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core.AttributeMagicalAttack
{
    [System.Serializable]
    public class MagicalAttack
    {
        private PlayerStatData PlayerStatData => PlayerStatData.Instance;
        private IMagicalAttack Owner {get; set;}
        [field: SerializeField] public BigNumber Base {get; private set;}
        [field: SerializeField] public BigNumber IntelligenceAttack {get; private set;}
        [ShowInInspector] public BigNumber Current => Base + IntelligenceAttack;
        public void Init(IMagicalAttack owner)
        {
            Owner = owner;
            IntelligenceAttack = 0;
            
            PlayerStatData[GameStat.Type.Intelligence].ReactiveLevel.ReactiveProperty
                .Subscribe(OnIntelligenceLevelChange)
                .AddTo(Owner as Component);
        }
        private void OnIntelligenceLevelChange(BigNumber intelligenceLevel)
        {
            IntelligenceAttack = Base * intelligenceLevel * 0.05f;
        }
    }
}