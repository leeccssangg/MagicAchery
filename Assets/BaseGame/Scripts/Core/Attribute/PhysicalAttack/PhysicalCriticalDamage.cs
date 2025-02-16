using R3;
using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core.AttributePhysicalAttack
{
    [System.Serializable]
    public class PhysicalCriticalDamage 
    {
        private PlayerStatData PlayerStatData => PlayerStatData.Instance;
        private TalentTreeManager TalentTreeManager => TalentTreeManager.Instance;
        private IPhysicalAttack Owner {get; set;}
        [field: SerializeField] public BigNumber Base {get; private set;}
        [field: SerializeField] public BigNumber AccuracyPhysicalCriticalDamage {get; private set;}
        [field: SerializeField] public BigNumber TalentPhysicalCriticalDamage {get; private set;}
        [ShowInInspector] public BigNumber Current => Base + AccuracyPhysicalCriticalDamage + TalentPhysicalCriticalDamage;
        public void Init(IPhysicalAttack owner)
        {
            Owner = owner;
            AccuracyPhysicalCriticalDamage = 0;
            TalentPhysicalCriticalDamage = 0;
            
            PlayerStatData[GameStat.Type.Accuracy].ReactiveLevel.ReactiveProperty
                .Subscribe(OnAccuracyLevelChange)
                .AddTo(Owner as Component);
            TalentTreeManager.GetTalentStat(TalentStat.Type.PhysicalCriticalDamage).ReactiveAmount.ReactiveProperty
                .Subscribe(OnTalentPhysicalAttackChange)
                .AddTo(Owner as Component);
        }
        private void OnAccuracyLevelChange(BigNumber accuracyLevel)
        {
            AccuracyPhysicalCriticalDamage = accuracyLevel * 0.5f;
        }
        private void OnTalentPhysicalAttackChange(BigNumber talentPhysicalAttack)
        {
            TalentPhysicalCriticalDamage = talentPhysicalAttack;
        }
    }
}