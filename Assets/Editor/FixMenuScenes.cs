#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EpochsOfWar.UI;

namespace EpochsOfWar.Editor
{
    /// <summary>
    /// Automatically fixes menu scenes by adding MenuBootstrapper and ensuring proper scaling
    /// </summary>
    public static class FixMenuScenes
    {
        [MenuItem("Tools/EpochsOfWar/Fix All Menu Scenes")]
        public static void FixAllMenuScenes()
        {
            string[] menuScenes = {
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/GameModeSelection.unity"
            };
            
            foreach (string scenePath in menuScenes)
            {
                if (System.IO.File.Exists(scenePath))
                {
                    FixMenuScene(scenePath);
                }
                else
                {
                    Debug.LogWarning($"Scene not found: {scenePath}");
                }
            }
            
            Debug.Log("✅ All menu scenes fixed and saved!");
        }
        
        [MenuItem("Tools/EpochsOfWar/Fix Current Scene")]
        public static void FixCurrentScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            if (string.IsNullOrEmpty(activeScene.path))
            {
                Debug.LogError("Current scene must be saved before fixing!");
                return;
            }
            
            FixMenuScene(activeScene.path);
            Debug.Log($"✅ Fixed scene: {activeScene.name}");
        }
        
        static void FixMenuScene(string scenePath)
        {
            // Open the scene
            var scene = EditorSceneManager.OpenScene(scenePath);
            
            // Find all Canvas objects
            var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            
            foreach (var canvas in canvases)
            {
                FixCanvas(canvas);
            }
            
            // Ensure there's a camera if none exists
            EnsureCamera();
            
            // Ensure EventSystem exists and is upgraded
            EnsureEventSystem();
            
            // Save the scene
            EditorSceneManager.SaveScene(scene);
        }
        
        static void FixCanvas(Canvas canvas)
        {
            // Add MenuBootstrapper if missing
            var bootstrapper = canvas.GetComponent<MenuBootstrapper>();
            if (bootstrapper == null)
            {
                bootstrapper = canvas.gameObject.AddComponent<MenuBootstrapper>();
                Debug.Log($"Added MenuBootstrapper to {canvas.name}");
            }
            
            // Ensure CanvasScaler exists
            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();
                Debug.Log($"Added CanvasScaler to {canvas.name}");
            }
            
            // Ensure GraphicRaycaster exists
            var raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                raycaster = canvas.gameObject.AddComponent<GraphicRaycaster>();
                Debug.Log($"Added GraphicRaycaster to {canvas.name}");
            }
            
            // Fix all Text components to use proper fonts and larger sizes
            FixTextComponents(canvas.transform);
            
            // Fix all Button components to have proper sizes
            FixButtonComponents(canvas.transform);
        }
        
        static void FixTextComponents(Transform parent)
        {
            var texts = parent.GetComponentsInChildren<Text>(true);
            foreach (var text in texts)
            {
                // Update font
                text.font = MenuBootstrapper.DefaultFont;
                
                // Increase font sizes that are too small
                if (text.fontSize < 18)
                {
                    text.fontSize = Mathf.Max(18, text.fontSize * 2);
                }
                
                // Ensure proper color
                if (text.color.a < 0.5f)
                {
                    text.color = Color.white;
                }
            }
        }
        
        static void FixButtonComponents(Transform parent)
        {
            var buttons = parent.GetComponentsInChildren<Button>(true);
            foreach (var button in buttons)
            {
                var rect = button.GetComponent<RectTransform>();
                
                // Ensure minimum button height
                if (rect.sizeDelta.y > 0 && rect.sizeDelta.y < 60)
                {
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, 60);
                }
                
                // Fix button text
                var buttonText = button.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.font = MenuBootstrapper.DefaultFont;
                    if (buttonText.fontSize < 20)
                    {
                        buttonText.fontSize = 20;
                    }
                }
            }
        }
        
        static void EnsureCamera()
        {
            var camera = Object.FindFirstObjectByType<Camera>();
            if (camera == null)
            {
                var cameraGO = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
                cameraGO.tag = "MainCamera";
                var cam = cameraGO.GetComponent<Camera>();
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.05f, 0.08f, 0.12f, 1f);
                cam.orthographic = false;
                cam.transform.position = new Vector3(0f, 0f, -10f);
                Debug.Log("Created Main Camera");
            }
        }
        
        static void EnsureEventSystem()
        {
            var eventSystem = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                var eventSystemGO = new GameObject("EventSystem");
                eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                Debug.Log("Created EventSystem with InputSystemUIInputModule");
            }
            else
            {
                // Upgrade to new Input System if needed
                var legacyModule = eventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                var newModule = eventSystem.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                
                if (legacyModule != null && newModule == null)
                {
                    Object.DestroyImmediate(legacyModule);
                    eventSystem.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                    Debug.Log("Upgraded EventSystem to InputSystemUIInputModule");
                }
            }
        }
    }
}
#endif
