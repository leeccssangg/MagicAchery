using R3;
using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core.AttributeMagicalAttack
{
    [System.Serializable]
    public class MagicalCriticalChance
    {
        private PlayerStatData PlayerStatData => PlayerStatData.Instance;
        private TalentTreeManager TalentTreeManager => TalentTreeManager.Instance;
        private IMagicalAttack Owner {get; set;}
        [field: SerializeField] public BigNumber Base {get; private set;}
        [field: SerializeField] public BigNumber AccuracyMagicalCriticalChance {get; private set;}
        [field: SerializeField] public BigNumber TalentMagicalCriticalChance {get; private set;}
        [ShowInInspector] public BigNumber Current => Base + AccuracyMagicalCriticalChance + TalentMagicalCriticalChance;
        public void Init(IMagicalAttack owner)
        {
            Owner = owner;
            AccuracyMagicalCriticalChance = 0;
            TalentMagicalCriticalChance = 0;
            
            PlayerStatData[GameStat.Type.Accuracy].ReactiveLevel.ReactiveProperty
                .Subscribe(OnAccuracyLevelChange)
                .AddTo(Owner as Component);
            TalentTreeManager.GetTalentStat(TalentStat.Type.MagicalCriticalChance).ReactiveAmount.ReactiveProperty
                .Subscribe(OnTalentMagicalAttackChange)
                .AddTo(Owner as Component);
        }
        private void OnAccuracyLevelChange(BigNumber accuracyLevel)
        {
            AccuracyMagicalCriticalChance = BigNumber.Min(accuracyLevel * 0.05f, 10);
        }
        private void OnTalentMagicalAttackChange(BigNumber talentMagicalAttack)
        {
            TalentMagicalCriticalChance = talentMagicalAttack;
        }
    }
}