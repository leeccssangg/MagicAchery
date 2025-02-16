using UnityEngine;
using UnityEngine.UI;

public class UIRawImageLoop : MonoBehaviour
{
    [field: SerializeField] public RawImage UIImage {get; private set;}
    [field: SerializeField] public float Speed {get; private set;}
    private Rect m_CurrentRect;
    private void Start()
    {
        m_CurrentRect = UIImage.uvRect;
    }
    private void Update()
    {
        m_CurrentRect.x += Speed * Time.deltaTime;
        m_CurrentRect.y += Speed * Time.deltaTime;
        UIImage.uvRect = m_CurrentRect;
    }
}