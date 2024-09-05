using UnityEditor;
using UnityEngine;

public static class SelectWebGlTemplate
{
    [MenuItem("Helpers/WebGLTemplate")]
    private static void SelectTemplate()
    {
        EditorUtility.FocusProjectWindow();
        Object _object = AssetDatabase.LoadAssetAtPath(
            AssetDatabase.GetAssetPath(Resources.Load<TextAsset>("WebTemplate/index")), 
            typeof(Object));
        Selection.activeObject = _object;
    }
}
