using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TMPro
{
    [CreateAssetMenu(fileName = "TMPStyleAssetGenerator", menuName = "TW/Utility/TMPStyleAssetGenerator")]

    public class TMPStyleAssetGenerator : ScriptableObject
    {
        [field: SerializeField] public TMP_StyleSheet TMP_StyleSheet { get; private set; }
        [field: SerializeField] public TMPStyleConfig[] TMPStyleConfigs { get; private set; }

        [Button]
        public void GenerateTMPStyleAsset()
        {
            SerializedObject serializedObject = new SerializedObject(TMP_StyleSheet);
            SerializedProperty styleList = serializedObject.FindProperty("m_StyleList");
            styleList.ClearArray();
            styleList.arraySize = TMPStyleConfigs.Length + 1;
            for (int i = 0; i < TMPStyleConfigs.Length; i++)
            {
                styleList.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue = TMPStyleConfigs[i].Style;
                styleList.GetArrayElementAtIndex(i).FindPropertyRelative("m_HashCode").intValue = TMP_TextParsingUtilities.GetHashCode(TMPStyleConfigs[i].Style);
                styleList.GetArrayElementAtIndex(i).FindPropertyRelative("m_OpeningDefinition").stringValue = $"<sprite index={TMPStyleConfigs[i].Index}>";
            }
            styleList.GetArrayElementAtIndex(TMPStyleConfigs.Length).FindPropertyRelative("m_Name").stringValue = "Normal";
            styleList.GetArrayElementAtIndex(TMPStyleConfigs.Length).FindPropertyRelative("m_HashCode").intValue = TMP_TextParsingUtilities.GetHashCode("Normal");
            styleList.serializedObject.ApplyModifiedProperties();
            
        }
    }
}
[System.Serializable]
public class TMPStyleConfig
{
    [field: SerializeField] public string Style {get; private set;}
    [field: SerializeField] public int Index {get; private set;}
}