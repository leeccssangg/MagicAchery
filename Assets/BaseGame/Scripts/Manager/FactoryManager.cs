using Core;
using Core.SimplePool;
using DamageNumbersPro;
using R3;
using R3.Triggers;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;

public class FactoryManager : Singleton<FactoryManager>
{
    [field: SerializeField] private Color[] TextColor {get; set;}
    [field: SerializeField] private DamageNumber DamageText {get; set;}
    [field: SerializeField] private DamageNumber CriticalDamageText {get; set;}
    [field: SerializeField] private DamageNumber ExperienceText {get; set;}
    [field: SerializeField] private VisualEffect ParalysisEffect {get; set;}
    [field: SerializeField] private VisualEffect BurnEffect {get; set;}

    public DamageNumber SpawnDamageText(BigNumber damage, DamageType damageType, Vector3 position, bool isCritical)
    {
        string text = $"{damage.ToStringUI()}{(isCritical ? "!" : "")}";
        Color color = TextColor[(int)damageType * 2 + (isCritical ? 1 : 0)];
        int fontSize = isCritical ? 8 : 6;
        DamageNumber textPrefab = isCritical ? CriticalDamageText : DamageText;
        DamageNumber damageNumber = textPrefab.Spawn(position, text);
        damageNumber.SetColor(color);
        damageNumber.GetTextMesh().fontSize = fontSize;
        
        return damageNumber;
    }
    public DamageNumber SpawnExperienceText(BigNumber experience, Vector3 position)
    {
        string text = $"+{experience.ToStringUI()}xp";
        DamageNumber damageNumber = ExperienceText.Spawn(position, text);
        return damageNumber;
    }
    public VisualEffect SpawnParalysisEffect(Vector3 position, Quaternion rotation, Transform parent)
    {
        VisualEffect paralysisEffect = ParalysisEffect.Spawn(position, rotation, parent);
        return paralysisEffect;
    }
    
    public VisualEffect SpawnBurnEffect(Vector3 position, Quaternion rotation, Transform parent)
    {
        VisualEffect burnEffect = BurnEffect.Spawn(position, rotation, parent);
        return burnEffect;
    }

    
    

}