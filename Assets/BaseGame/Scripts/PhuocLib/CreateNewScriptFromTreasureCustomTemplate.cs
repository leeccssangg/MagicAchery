#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateNewScriptFromTreasureCustomTemplate 
{
#if UNITY_EDITOR
    [MenuItem(itemName: "Assets/CustomTemplate/Treasure Config/Create New Treasure Config", isValidateFunction: false, priority: 1)]
    public static void CreateScriptScriptableObjectFromTemplate()
    {
        CreateScriptFromTemplate("NewTreasureConfigTemplate.cs", "NewTreasureConfig.cs");
    }
    public static void CreateScriptFromTemplate(string templateName, string defaultFileName)
    {
        string[] findAssets = AssetDatabase.FindAssets($"t:TextAsset {templateName}");

        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(AssetDatabase.GUIDToAssetPath(findAssets[0]), defaultFileName);
    }
#endif
}
