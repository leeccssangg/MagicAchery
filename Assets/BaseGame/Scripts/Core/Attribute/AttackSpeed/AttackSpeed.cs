using Sirenix.OdinInspector;
using UnityEngine;
using R3;
using TW.Utility.CustomType;

namespace Core.AttributeAttackSpeed
{
    [System.Serializable]
    public class AttackSpeed
    {
        private PlayerStatData PlayerStatData => PlayerStatData.Instance;
        private TalentTreeManager TalentTreeManager => TalentTreeManager.Instance;
        private IAttackSpeed Owner {get; set;}
        [field: SerializeField] public float Base {get; private set;}
        [field: SerializeField] public float DexterityAttackSpeed {get; private set;}
        [field: SerializeField] public float TalentAttackSpeed {get; private set;}
        [field: SerializeField] public float OtherAttackSpeed {get; private set;}
        [ShowInInspector] public float Current => Base + TalentAttackSpeed + DexterityAttackSpeed + OtherAttackSpeed;
        
        public void Init(IAttackSpeed owner)
        {
            Owner = owner;
            DexterityAttackSpeed = 0;
            TalentAttackSpeed = 0;
            OtherAttackSpeed = 0;
            
            PlayerStatData[GameStat.Type.Dexterity].ReactiveLevel.ReactiveProperty
                .Subscribe(OnDexterityLevelChange)
                .AddTo(Owner as Component);
            TalentTreeManager.GetTalentStat(TalentStat.Type.AttackSpeed).ReactiveAmount.ReactiveProperty
                .Subscribe(OnTalentAttackSpeedChange)
                .AddTo(Owner as Component);
        }
        private void OnDexterityLevelChange(BigNumber dexterityLevel)
        {
            DexterityAttackSpeed = Base * dexterityLevel.ToFloat() * 0.05f;
        }

        private void OnTalentAttackSpeedChange(BigNumber talentAttackSpeed)
        {
            TalentAttackSpeed = talentAttackSpeed.ToFloat() / 100f;
        }
        public void ChangeOtherAttackSpeed(float value)
        {
            OtherAttackSpeed += value / 100f;
        }
    }
}