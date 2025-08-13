#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class FontDebugger
{
    [MenuItem("Tools/Debug/Test Font Loading")]
    public static void TestFontLoading()
    {
        Debug.Log("=== Font Loading Test ===");
        
        // Test 1: AssetDatabase.GetBuiltinExtraResource
        try
        {
            var font1 = AssetDatabase.GetBuiltinExtraResource<Font>("LegacyRuntime.ttf");
            Debug.Log($"AssetDatabase.GetBuiltinExtraResource LegacyRuntime: {(font1 != null ? font1.name : "NULL")}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AssetDatabase.GetBuiltinExtraResource LegacyRuntime failed: {e.Message}");
        }
        
        // Test 2: EditorGUIUtility.Load
        try
        {
            var font2 = EditorGUIUtility.Load("LegacyRuntime.ttf") as Font;
            Debug.Log($"EditorGUIUtility.Load LegacyRuntime: {(font2 != null ? font2.name : "NULL")}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"EditorGUIUtility.Load LegacyRuntime failed: {e.Message}");
        }
        
        // Test 3: Try with Arial path as fallback
        try
        {
            var font3 = AssetDatabase.GetBuiltinExtraResource<Font>("Arial.ttf");
            Debug.Log($"AssetDatabase.GetBuiltinExtraResource Arial: {(font3 != null ? font3.name : "NULL")}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AssetDatabase.GetBuiltinExtraResource Arial failed: {e.Message}");
        }
        
        // Test 4: Dynamic font creation
        try
        {
            var font4 = Font.CreateDynamicFontFromOSFont("Arial", 16);
            Debug.Log($"CreateDynamicFontFromOSFont Arial: {(font4 != null ? font4.name : "NULL")}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"CreateDynamicFontFromOSFont Arial failed: {e.Message}");
        }
        
        // Test 5: GUI.skin.font
        try
        {
            var font5 = GUI.skin.font;
            Debug.Log($"GUI.skin.font: {(font5 != null ? font5.name : "NULL")}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GUI.skin.font failed: {e.Message}");
        }
        
        Debug.Log("=== End Font Test ===");
    }
}
#endif
