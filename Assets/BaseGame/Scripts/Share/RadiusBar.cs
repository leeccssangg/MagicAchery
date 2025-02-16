using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusBar : MonoBehaviour
{
    private static readonly int FillAmount = Shader.PropertyToID("_Arc1");
    [field: SerializeField] public SpriteRenderer FillSprite {get; private set;}
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
    public void SetProgress(float fillAmount)
    {
        FillSprite.material.SetFloat(FillAmount, (1 - fillAmount) * 360f);
    }
}
