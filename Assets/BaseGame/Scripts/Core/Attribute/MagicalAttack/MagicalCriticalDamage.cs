using R3;
using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core.AttributeMagicalAttack
{
    [System.Serializable]
    public class MagicalCriticalDamage
    {
        private PlayerStatData PlayerStatData => PlayerStatData.Instance;
        private TalentTreeManager TalentTreeManager => TalentTreeManager.Instance;
        private IMagicalAttack Owner {get; set;}
        [field: SerializeField] public BigNumber Base {get; private set;}
        [field: SerializeField] public BigNumber AccuracyMagicalCriticalDamage {get; private set;}
        [field: SerializeField] public BigNumber TalentMagicalCriticalDamage {get; private set;}
        [ShowInInspector] public BigNumber Current => Base + AccuracyMagicalCriticalDamage + TalentMagicalCriticalDamage;
        public void Init(IMagicalAttack owner)
        {
            Owner = owner;
            AccuracyMagicalCriticalDamage = 0;
            TalentMagicalCriticalDamage = 0;
            
            PlayerStatData[GameStat.Type.Accuracy].ReactiveLevel.ReactiveProperty
                .Subscribe(OnAccuracyLevelChange)
                .AddTo(Owner as Component);
            TalentTreeManager.GetTalentStat(TalentStat.Type.MagicalCriticalDamage).ReactiveAmount.ReactiveProperty
                .Subscribe(OnTalentMagicalAttackChange)
                .AddTo(Owner as Component);
        }
        private void OnAccuracyLevelChange(BigNumber accuracyLevel)
        {
            AccuracyMagicalCriticalDamage = accuracyLevel * 0.5f;
        }
        private void OnTalentMagicalAttackChange(BigNumber talentMagicalAttack)
        {
            TalentMagicalCriticalDamage = talentMagicalAttack;
        }
    }
}