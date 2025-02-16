using TW.Utility.CustomComponent;
using UnityEngine;

public class ProgressBar : ACachedMonoBehaviour
{
    private static readonly int FillAmount = Shader.PropertyToID("_FillAmount");
    [field: SerializeField] public SpriteRenderer FillSprite {get; private set;}
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
    public void SetProgress(float fillAmount)
    {
        FillSprite.material.SetFloat(FillAmount, fillAmount);
    }
}