using R3;
using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core.AttributePhysicalAttack
{
    [System.Serializable]
    public class PhysicalAttack
    {
        private PlayerStatData PlayerStatData => PlayerStatData.Instance;
        private TalentTreeManager TalentTreeManager => TalentTreeManager.Instance;
        private IPhysicalAttack Owner {get; set;}
        [field: SerializeField] public BigNumber Base {get; private set;}
        [field: SerializeField] public BigNumber StrengthAttack {get; private set;}
        [field: SerializeField] public BigNumber TalentAttack {get; private set;}
        [ShowInInspector] public BigNumber Current => Base + StrengthAttack + TalentAttack;
        public void Init(IPhysicalAttack owner)
        {
            Owner = owner;
            PlayerStatData[GameStat.Type.Strength].ReactiveLevel.ReactiveProperty
                .Subscribe(OnStrengthLevelChange)
                .AddTo(Owner as Component);
            TalentTreeManager.GetTalentStat(TalentStat.Type.PhysicalAttackDamage).ReactiveAmount.ReactiveProperty
                .Subscribe(OnTalentPhysicalAttackChange)
                .AddTo(Owner as Component);
        }
        public void OnStrengthLevelChange(BigNumber strengthLevel)
        {
            StrengthAttack = Base * strengthLevel * 0.05f;
        }
        private void OnTalentPhysicalAttackChange(BigNumber talentPhysicalAttack)
        {
            TalentAttack = talentPhysicalAttack;
        }
    }
}