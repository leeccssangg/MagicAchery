using TMPro;
using TW.Utility.CustomType;
using UnityEngine;

public class UIAimAngle : MonoBehaviour
{
    [field: SerializeField] public TextMeshProUGUI TextAngle {get; private set;}
    [field: SerializeField] public RectTransform Angle {get; private set;}
    [field: SerializeField] public FloatRange AngleRange {get; private set;}
    [field: SerializeField] public FloatRange AnglePosition {get; private set;}
    private string AngleFormat { get; set; } = "{0}<size=25><voffset=0.75em>o";
    public void SetAngle(float angle)
    {
        TextAngle.text = string.Format(AngleFormat, angle.ToString("F1"));
        float alpha = Mathf.InverseLerp(AngleRange.m_Min, AngleRange.m_Max, angle);
        Angle.localPosition = new Vector3(0, Mathf.Lerp(AnglePosition.m_Min, AnglePosition.m_Max, alpha), 0);
    }
}