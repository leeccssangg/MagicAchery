using R3;
using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core.AttributePhysicalAttack
{
    [System.Serializable]
    public class PhysicalCriticalChance
    {
        private PlayerStatData PlayerStatData => PlayerStatData.Instance;
        private TalentTreeManager TalentTreeManager => TalentTreeManager.Instance;
        private IPhysicalAttack Owner {get; set;}
        [field: SerializeField] public BigNumber Base {get; private set;}
        [field: SerializeField] public BigNumber AccuracyPhysicalCriticalChance {get; private set;}
        [field: SerializeField] public BigNumber TalentPhysicalCriticalChance {get; private set;}
        [ShowInInspector] public BigNumber Current => Base + AccuracyPhysicalCriticalChance + TalentPhysicalCriticalChance;
        public void Init(IPhysicalAttack owner)
        {
            Owner = owner;
            AccuracyPhysicalCriticalChance = 0;
            TalentPhysicalCriticalChance = 0;
            
            PlayerStatData[GameStat.Type.Accuracy].ReactiveLevel.ReactiveProperty
                .Subscribe(OnAccuracyLevelChange)
                .AddTo(Owner as Component);
            TalentTreeManager.GetTalentStat(TalentStat.Type.PhysicalCriticalChance).ReactiveAmount.ReactiveProperty
                .Subscribe(OnTalentPhysicalAttackChange)
                .AddTo(Owner as Component);
        }
        private void OnAccuracyLevelChange(BigNumber accuracyLevel)
        {
            AccuracyPhysicalCriticalChance = BigNumber.Min(accuracyLevel * 0.05f, 10);
        }
        private void OnTalentPhysicalAttackChange(BigNumber talentPhysicalAttack)
        {
            TalentPhysicalCriticalChance = talentPhysicalAttack;
        }
    }
}