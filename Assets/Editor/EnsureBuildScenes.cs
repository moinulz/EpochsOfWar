#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace EpochsOfWar.Editor
{
    /// <summary>
    /// Ensures build settings are properly configured with the required scenes in the correct order
    /// </summary>
    public static class EnsureBuildScenes
    {
        [MenuItem("Tools/EpochsOfWar/Setup Build Settings")]
        public static void SetupBuildSettings()
        {
            // Define required scenes in build order
            string[] requiredScenes = {
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/GameModeSelection.unity", 
                "Assets/Scenes/Game.unity"
            };
            
            // Get current build settings
            var currentScenes = EditorBuildSettings.scenes.ToList();
            var newScenes = new List<EditorBuildSettingsScene>();
            
            // Add required scenes first
            foreach (string scenePath in requiredScenes)
            {
                if (System.IO.File.Exists(scenePath))
                {
                    newScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                    Debug.Log($"✅ Added to build: {scenePath}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ Required scene not found: {scenePath}");
                }
            }
            
            // Handle SampleScene specially - keep on disk but remove from build
            string sampleScenePath = "Assets/Scenes/SampleScene.unity";
            if (System.IO.File.Exists(sampleScenePath))
            {
                // Don't add to build settings, but log that we're keeping it
                Debug.Log($"📁 Keeping on disk but not in build: {sampleScenePath}");
            }
            
            // Add any other existing scenes that aren't in our required list or SampleScene
            foreach (var existingScene in currentScenes)
            {
                if (!requiredScenes.Contains(existingScene.path) && 
                    existingScene.path != sampleScenePath &&
                    !newScenes.Any(s => s.path == existingScene.path))
                {
                    newScenes.Add(existingScene);
                    Debug.Log($"📋 Preserved existing scene: {existingScene.path}");
                }
            }
            
            // Apply new build settings
            EditorBuildSettings.scenes = newScenes.ToArray();
            
            // Report final state
            Debug.Log($"🎯 Build Settings Updated!");
            Debug.Log($"📊 Total scenes in build: {newScenes.Count}");
            for (int i = 0; i < newScenes.Count; i++)
            {
                Debug.Log($"   {i}: {newScenes[i].path} (enabled: {newScenes[i].enabled})");
            }
            
            // Validate scene order
            ValidateSceneOrder(newScenes);
        }
        
        [MenuItem("Tools/EpochsOfWar/Validate Build Settings")]
        public static void ValidateBuildSettings()
        {
            var scenes = EditorBuildSettings.scenes.ToList();
            ValidateSceneOrder(scenes);
        }
        
        static void ValidateSceneOrder(List<EditorBuildSettingsScene> scenes)
        {
            string[] expectedOrder = {
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/GameModeSelection.unity",
                "Assets/Scenes/Game.unity"
            };
            
            bool orderCorrect = true;
            
            for (int i = 0; i < expectedOrder.Length && i < scenes.Count; i++)
            {
                if (scenes[i].path != expectedOrder[i])
                {
                    orderCorrect = false;
                    break;
                }
            }
            
            if (orderCorrect)
            {
                Debug.Log("✅ Scene build order is correct!");
            }
            else
            {
                Debug.LogWarning("⚠️ Scene build order may be incorrect. Expected order:");
                for (int i = 0; i < expectedOrder.Length; i++)
                {
                    Debug.LogWarning($"   {i}: {expectedOrder[i]}");
                }
            }
        }
        
        [MenuItem("Tools/EpochsOfWar/Remove SampleScene from Build")]
        public static void RemoveSampleSceneFromBuild()
        {
            var scenes = EditorBuildSettings.scenes.ToList();
            string sampleScenePath = "Assets/Scenes/SampleScene.unity";
            
            bool removed = scenes.RemoveAll(s => s.path == sampleScenePath) > 0;
            
            if (removed)
            {
                EditorBuildSettings.scenes = scenes.ToArray();
                Debug.Log($"🗑️ Removed SampleScene from build settings (kept on disk)");
            }
            else
            {
                Debug.Log($"ℹ️ SampleScene was not in build settings");
            }
        }
        
        [MenuItem("Tools/EpochsOfWar/List All Scenes")]
        public static void ListAllScenes()
        {
            Debug.Log("📋 All scenes in project:");
            
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
            foreach (string guid in sceneGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                bool inBuild = EditorBuildSettings.scenes.Any(s => s.path == path && s.enabled);
                string status = inBuild ? "🎯 IN BUILD" : "📁 ON DISK";
                Debug.Log($"   {status}: {path}");
            }
            
            Debug.Log($"\n🎯 Build Settings ({EditorBuildSettings.scenes.Length} scenes):");
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var scene = EditorBuildSettings.scenes[i];
                string status = scene.enabled ? "✅" : "❌";
                Debug.Log($"   {i}: {status} {scene.path}");
            }
        }
    }
}
#endif
