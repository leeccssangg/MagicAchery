using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

#endif
public partial class UILinkNode : ACachedMonoBehaviour
{
    [field: SerializeField] public RectTransform RectImage {get; private set;}
    
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
    
}

#if UNITY_EDITOR
public partial class UILinkNode : ACachedMonoBehaviour
{
    [Button]
    public void Setup(Transform start, Transform end)
    {
        EditorUtility.SetDirty(Transform);
        EditorUtility.SetDirty(RectImage);
        
        Vector3 startPos = start.position;
        Vector3 endPos = end.position;
        Vector3 dir = (endPos - startPos).normalized;
        float distance = Vector3.Distance(startPos, endPos);
        Transform.position = startPos + dir * distance / 2;
        RectImage.sizeDelta = new Vector2(distance, RectImage.sizeDelta.y);
        RectImage.rotation = Quaternion.LookRotation(dir, Vector3.forward) * Quaternion.Euler(90, 0, 90);
    }
    
}
#endif