using UnityEngine;
using UnityEngine.UI;

namespace EpochsOfWar.UI
{
    /// <summary>
    /// Ensures proper UI scaling and font setup across all menu scenes.
    /// Attach this to Canvas objects to automatically configure responsive scaling.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class MenuBootstrapper : MonoBehaviour
    {
        [Header("Scaling Configuration")]
        [SerializeField] private Vector2 referenceResolution = new Vector2(1920, 1080);
        [SerializeField] private float matchWidthOrHeight = 0.5f;
        [SerializeField] private int fallbackDPI = 96;
        
        [Header("Font Settings")]
        [SerializeField] private string defaultFontName = "Arial";
        [SerializeField] private int defaultFontSize = 16;
        
        private Canvas canvas;
        private CanvasScaler canvasScaler;
        private static Font _defaultFont;
        
        public static Font DefaultFont
        {
            get
            {
                if (_defaultFont == null)
                {
                    _defaultFont = Font.CreateDynamicFontFromOSFont("Arial", 16);
                }
                return _defaultFont;
            }
        }
        
        void Awake()
        {
            SetupCanvas();
            SetupScaling();
            EnsureEventSystem();
        }
        
        void SetupCanvas()
        {
            canvas = GetComponent<Canvas>();
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            canvas.sortingOrder = 0;
        }
        
        void SetupScaling()
        {
            canvasScaler = GetComponent<CanvasScaler>();
            if (canvasScaler == null)
            {
                canvasScaler = gameObject.AddComponent<CanvasScaler>();
            }
            
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = referenceResolution;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
            canvasScaler.physicalUnit = CanvasScaler.Unit.Points;
            canvasScaler.fallbackScreenDPI = fallbackDPI;
            canvasScaler.defaultSpriteDPI = fallbackDPI;
        }
        
        void EnsureEventSystem()
        {
            var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                var eventSystemGO = new GameObject("EventSystem");
                eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                Debug.Log("MenuBootstrapper: Created EventSystem with InputSystemUIInputModule");
            }
            else
            {
                // Ensure we're using the new Input System UI module
                var legacyModule = eventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                var newModule = eventSystem.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                
                if (legacyModule != null && newModule == null)
                {
                    DestroyImmediate(legacyModule);
                    eventSystem.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                    Debug.Log("MenuBootstrapper: Upgraded EventSystem to InputSystemUIInputModule");
                }
            }
        }
        
        /// <summary>
        /// Creates properly scaled UI text with the default font
        /// </summary>
        public static Text CreateScaledText(Transform parent, string name, string text, int fontSize = 24)
        {
            var textGO = new GameObject(name, typeof(Text));
            textGO.transform.SetParent(parent, false);
            
            var textComponent = textGO.GetComponent<Text>();
            textComponent.text = text;
            textComponent.font = DefaultFont;
            textComponent.fontSize = fontSize;
            textComponent.color = Color.white;
            textComponent.alignment = TextAnchor.MiddleCenter;
            
            return textComponent;
        }
        
        /// <summary>
        /// Creates properly scaled UI button with default styling
        /// </summary>
        public static Button CreateScaledButton(Transform parent, string name, string text, int fontSize = 20)
        {
            var buttonGO = new GameObject(name, typeof(Image), typeof(Button));
            buttonGO.transform.SetParent(parent, false);
            
            var image = buttonGO.GetComponent<Image>();
            image.color = new Color(0.2f, 0.3f, 0.6f, 1f);
            
            var button = buttonGO.GetComponent<Button>();
            
            // Create button text
            var textGO = new GameObject("Text", typeof(Text));
            textGO.transform.SetParent(buttonGO.transform, false);
            
            var textComponent = textGO.GetComponent<Text>();
            textComponent.text = text;
            textComponent.font = DefaultFont;
            textComponent.fontSize = fontSize;
            textComponent.color = Color.white;
            textComponent.alignment = TextAnchor.MiddleCenter;
            
            // Set text to fill button
            var textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            return button;
        }
    }
}