#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.UI;

public static class CreateMainMenuScene
{
    [MenuItem("Tools/Project Setup/Upgrade EventSystems to Input System")]
    public static void UpgradeEventSystemsInOpenScenes()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;
            foreach (var root in scene.GetRootGameObjects())
            {
                var eventSystems = root.GetComponentsInChildren<UnityEngine.EventSystems.EventSystem>(true);
                foreach (var ev in eventSystems)
                {
                    var go = ev.gameObject;
                    var legacy = go.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                    var newMod = go.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                    if (legacy != null)
                    {
                        Object.DestroyImmediate(legacy, true);
                        if (newMod == null) go.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                        EditorSceneManager.MarkSceneDirty(scene);
                    }
                }
            }
        }
        EditorUtility.DisplayDialog("Upgrade Complete", "Replaced StandaloneInputModule with InputSystemUIInputModule in loaded scenes.", "OK");
    }
    [MenuItem("Tools/Project Setup/Create Main Menu %#m")] // Ctrl/Cmd+Shift+M
    public static void CreateMenuAndScenes()
    {
        // Create MainMenu scene
        var mainScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        mainScene.name = "MainMenu";

        // EventSystem
        var es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        // --- Ensure there is a camera in the scene ---
var camGO = new GameObject("Main Camera");
var cam = camGO.AddComponent<Camera>();
camGO.AddComponent<AudioListener>();
camGO.tag = "MainCamera";
// Good defaults for menu
cam.clearFlags = CameraClearFlags.SolidColor;
cam.backgroundColor = new Color(0.05f, 0.08f, 0.12f, 1f);
cam.orthographic = false;
cam.transform.position = new Vector3(0f, 0f, -10f);
cam.transform.rotation = Quaternion.identity;

        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        UIDesignSystem.SetupResponsiveCanvas(scaler);

        // Background with modern gradient effect
        var bg = new GameObject("Background", typeof(Image));
        bg.transform.SetParent(canvasGO.transform, false);
        var bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero; 
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;  
        bgRect.offsetMax = Vector2.zero;
        bg.GetComponent<Image>().color = UIDesignSystem.Colors.PrimaryDark;

        // Title with improved typography
        var title = new GameObject("Title", typeof(Text));
        title.transform.SetParent(canvasGO.transform, false);
        var titleTxt = title.GetComponent<Text>();
        titleTxt.text = "EPOCHS OF WAR";
        titleTxt.alignment = TextAnchor.MiddleCenter;
        titleTxt.font = GetDefaultFont();
        titleTxt.fontSize = UIDesignSystem.Typography.TitleLarge;
        titleTxt.fontStyle = FontStyle.Bold;
        titleTxt.color = UIDesignSystem.Colors.AccentGoldLight;
        
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.9f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(800f, 100f);

        var subtitle = new GameObject("Subtitle", typeof(Text));
        subtitle.transform.SetParent(canvasGO.transform, false);
        var subtitleTxt = subtitle.GetComponent<Text>();
        subtitleTxt.text = "FORGE YOUR EMPIRE • COMMAND YOUR ARMIES • CONQUER THE AGES";
        subtitleTxt.alignment = TextAnchor.MiddleCenter;
        subtitleTxt.font = GetDefaultFont();
        subtitleTxt.fontSize = UIDesignSystem.Typography.BodyMedium;
        subtitleTxt.color = UIDesignSystem.Colors.TextSecondary;
        
        var subtitleRect = subtitle.GetComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0.5f, 0.75f);
        subtitleRect.anchorMax = new Vector2(0.5f, 0.8f);
        subtitleRect.pivot = new Vector2(0.5f, 0.5f);
        subtitleRect.anchoredPosition = Vector2.zero;
        subtitleRect.sizeDelta = new Vector2(600f, 40f);

        var buttonContainer = new GameObject("ButtonContainer", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        buttonContainer.transform.SetParent(canvasGO.transform, false);
        var containerRect = buttonContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.3f);
        containerRect.anchorMax = new Vector2(0.5f, 0.7f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = Vector2.zero;

        var vlg = buttonContainer.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = UIDesignSystem.Spacing.Large;
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.childControlWidth = false;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = false;
        vlg.childForceExpandHeight = false;

        var fitter = buttonContainer.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Create modern buttons
        var playBtn = UIDesignSystem.ButtonFactory.CreatePrimaryButton(buttonContainer.transform, "PlayButton", "⚔ PLAY ⚔");
        var settingsBtn = UIDesignSystem.ButtonFactory.CreateSecondaryButton(buttonContainer.transform, "SettingsButton", "⚙ SETTINGS");
        var quitBtn = UIDesignSystem.ButtonFactory.CreateSecondaryButton(buttonContainer.transform, "QuitButton", "✕ QUIT");

        // Modern settings overlay
        var settingsPanel = new GameObject("SettingsPanel", typeof(Image));
        settingsPanel.transform.SetParent(canvasGO.transform, false);
        var spImg = settingsPanel.GetComponent<Image>();
        spImg.color = new Color(0f, 0f, 0f, 0.8f);
        var spRect = settingsPanel.GetComponent<RectTransform>();
        spRect.anchorMin = Vector2.zero; 
        spRect.anchorMax = Vector2.one;
        spRect.offsetMin = Vector2.zero; 
        spRect.offsetMax = Vector2.zero;
        settingsPanel.SetActive(false);

        // Settings content panel
        var panel = new GameObject("SettingsContent", typeof(Image));
        panel.transform.SetParent(settingsPanel.transform, false);
        panel.GetComponent<Image>().color = UIDesignSystem.Colors.SecondaryDark;
        var pRect = panel.GetComponent<RectTransform>();
        pRect.anchorMin = new Vector2(0.2f, 0.2f);
        pRect.anchorMax = new Vector2(0.8f, 0.8f);
        pRect.offsetMin = Vector2.zero;
        pRect.offsetMax = Vector2.zero;

        // Settings border
        var settingsBorder = new GameObject("Border", typeof(Image));
        settingsBorder.transform.SetParent(panel.transform, false);
        var borderImg = settingsBorder.GetComponent<Image>();
        borderImg.color = UIDesignSystem.Colors.AccentGold;
        var borderRect = settingsBorder.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = Vector2.zero;
        borderRect.offsetMax = Vector2.zero;
        
        var innerPanel = new GameObject("InnerPanel", typeof(Image));
        innerPanel.transform.SetParent(settingsBorder.transform, false);
        var innerImg = innerPanel.GetComponent<Image>();
        innerImg.color = UIDesignSystem.Colors.SecondaryDark;
        var innerRect = innerPanel.GetComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero;
        innerRect.anchorMax = Vector2.one;
        innerRect.offsetMin = new Vector2(4f, 4f);
        innerRect.offsetMax = new Vector2(-4f, -4f);

        // Settings content layout
        var settingsContent = new GameObject("SettingsLayout", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        settingsContent.transform.SetParent(innerPanel.transform, false);
        var contentRect = settingsContent.GetComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.offsetMin = new Vector2(UIDesignSystem.Spacing.XLarge, UIDesignSystem.Spacing.XLarge);
        contentRect.offsetMax = new Vector2(-UIDesignSystem.Spacing.XLarge, -UIDesignSystem.Spacing.XLarge);
        
        var settingsVlg = settingsContent.GetComponent<VerticalLayoutGroup>();
        settingsVlg.spacing = UIDesignSystem.Spacing.Large;
        settingsVlg.childControlWidth = true;
        settingsVlg.childControlHeight = false;
        settingsVlg.childForceExpandWidth = true;

        // Settings title
        var settingsTitle = new GameObject("SettingsTitle", typeof(Text));
        settingsTitle.transform.SetParent(settingsContent.transform, false);
        var stText = settingsTitle.GetComponent<Text>();
        stText.text = "⚙ SETTINGS ⚙";
        stText.alignment = TextAnchor.MiddleCenter;
        stText.font = GetDefaultFont();
        stText.fontSize = UIDesignSystem.Typography.TitleMedium;
        stText.fontStyle = FontStyle.Bold;
        stText.color = UIDesignSystem.Colors.AccentGoldLight;
        var stLayout = settingsTitle.AddComponent<LayoutElement>();
        stLayout.preferredHeight = 60f;

        // Settings controls with improved styling
        var qualityDropdown = UIDesignSystem.CreateStyledSettingsDropdown(settingsContent.transform, "Graphics Quality", new[] { "Low", "Medium", "High", "Ultra" });
        var musicSlider = UIDesignSystem.CreateStyledSettingsSlider(settingsContent.transform, "Music Volume", 0f, 1f, 0.5f);
        var vibToggle = UIDesignSystem.CreateStyledSettingsToggle(settingsContent.transform, "Vibration (Mobile)", true);

        // Close button
        var closeBtn = UIDesignSystem.ButtonFactory.CreateSecondaryButton(settingsContent.transform, "CloseButton", "✕ CLOSE");
        var closeBtnLayout = closeBtn.gameObject.AddComponent<LayoutElement>();
        closeBtnLayout.preferredHeight = 60f;
        closeBtnLayout.preferredWidth = 200f;

        // Loading overlay
        var loading = new GameObject("LoadingOverlay", typeof(CanvasGroup));
        loading.transform.SetParent(canvasGO.transform, false);
        var lg = loading.GetComponent<CanvasGroup>();
        lg.alpha = 0f; loading.SetActive(false);
        var lbg = new GameObject("Dim", typeof(Image));
        lbg.transform.SetParent(loading.transform, false);
        var lbgRect = lbg.GetComponent<RectTransform>();
        lbgRect.anchorMin = Vector2.zero; lbgRect.anchorMax = Vector2.one;
        lbgRect.offsetMin = Vector2.zero; lbgRect.offsetMax = Vector2.zero;
        lbg.GetComponent<Image>().color = new Color(0,0,0,0.75f);

        // Progress
        var prog = new GameObject("Progress", typeof(Slider));
        prog.transform.SetParent(loading.transform, false);
        var progRect = prog.GetComponent<RectTransform>();
        progRect.sizeDelta = new Vector2(800, 40);
        progRect.anchoredPosition = new Vector2(0, -100);
        var progLabel = new GameObject("Label", typeof(Text));
        progLabel.transform.SetParent(loading.transform, false);
        var pl = progLabel.GetComponent<Text>();
        pl.text = "0%";
        pl.alignment = TextAnchor.MiddleCenter;
    pl.font = GetDefaultFont();
        pl.color = Color.white;
        var plRect = progLabel.GetComponent<RectTransform>();
        plRect.sizeDelta = new Vector2(300, 80);
        plRect.anchoredPosition = new Vector2(0, -40);

        // Attach behaviour scripts
        var loader = canvasGO.AddComponent<SceneLoader>();
        loader.loadingGroup = lg;
        loader.progressBar = prog.GetComponent<Slider>();
        loader.progressLabel = pl;

        var settingsMgr = canvasGO.AddComponent<SettingsManager>();
        settingsMgr.qualityDropdown = qualityDropdown;
        settingsMgr.musicSlider = musicSlider;
        settingsMgr.vibrationToggle = vibToggle;

        var menu = canvasGO.AddComponent<MainMenu>();
        menu.playButton = playBtn;
        menu.settingsButton = settingsBtn;
        menu.quitButton = quitBtn;
        menu.settingsPanel = settingsPanel;
        menu.sceneLoader = loader;

        // Wire up close button - let MainMenu script handle settings button
        closeBtn.onClick.AddListener(() => settingsPanel.SetActive(false));

        // Save MainMenu scene
        var mmPath = "Assets/Scenes/MainMenu.unity";
        System.IO.Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), mmPath);

        // Create Game scene with ground and capital
        var gameScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        
        // Create EventSystem for UI
        var gameEventSystem = new GameObject("EventSystem");
        gameEventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        gameEventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        
        // Setup main camera for RTS-style view
        var mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.transform.position = new Vector3(0, 20, -12); // RTS camera position
            mainCam.transform.rotation = Quaternion.Euler(45, 0, 0); // RTS angle (45 degrees)
            mainCam.fieldOfView = 60f; // Good field of view for RTS
            mainCam.farClipPlane = 1000f; // Ensure we can see far objects
            mainCam.gameObject.AddComponent<CameraController>();
        }
        
        // Create ground plane with multiple quads for better terrain
        var terrainParent = new GameObject("Terrain");
        CreateRTSTerrain(terrainParent.transform);
        
        // Try to place Capital prefab
        var capitalPrefabPath = "Assets/Prefabs/Buildings/Capital.prefab";
        var capitalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(capitalPrefabPath);
        if (capitalPrefab != null)
        {
            var capital = PrefabUtility.InstantiatePrefab(capitalPrefab) as GameObject;
            capital.transform.position = new Vector3(0, 5f, 0); // Place above ground (y=5 to account for scaling)
            capital.transform.localScale = new Vector3(10, 10, 10); // Make it much larger
            capital.name = "Capital";
        }
        else
        {
            // Fallback: create a large cube if prefab not found
            var capital = GameObject.CreatePrimitive(PrimitiveType.Cube);
            capital.name = "Capital (Placeholder)";
            capital.transform.position = new Vector3(0, 5f, 0); // Elevated so it's visible on ground
            capital.transform.localScale = new Vector3(10, 10, 10); // 10x10x10 units
            var capitalMaterial = new Material(Shader.Find("Standard"));
            capitalMaterial.color = new Color(0.8f, 0.6f, 0.2f, 1f); // Golden color
            capital.GetComponent<Renderer>().material = capitalMaterial;
        }
        
        // Add some reference objects to help with visibility
        for (int i = 0; i < 4; i++)
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = $"Marker_{i}";
            float angle = i * 90f * Mathf.Deg2Rad;
            marker.transform.position = new Vector3(Mathf.Sin(angle) * 20f, 2f, Mathf.Cos(angle) * 20f);
            marker.transform.localScale = new Vector3(2, 4, 2);
            var markerMaterial = new Material(Shader.Find("Standard"));
            markerMaterial.color = new Color(1f, 0.2f, 0.2f, 1f); // Red markers
            marker.GetComponent<Renderer>().material = markerMaterial;
        }
        
        // Create UI Canvas for mobile controls
        var gameCanvas = new GameObject("GameUI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
    var gameCanvasComp = gameCanvas.GetComponent<Canvas>();
        gameCanvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
        var gameScaler = gameCanvas.GetComponent<CanvasScaler>();
    gameScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    gameScaler.referenceResolution = new Vector2(1920, 1080);
        
        // Note: Mobile control buttons removed - using keyboard controls only
        
        // Add Game Manager
        var gameManager = new GameObject("GameManager");
        var gameMgr = gameManager.AddComponent<GameManager>();
        if (mainCam != null)
        {
            gameMgr.gameCamera = mainCam;
            gameMgr.cameraController = mainCam.gameObject.GetComponent<CameraController>();
        }
        
        // Add Terrain Manager for grid-based building system
        var terrainMgr = terrainParent.AddComponent<TerrainManager>();
        terrainMgr.gridSize = 2f;
        terrainMgr.gridWidth = 50;
        terrainMgr.gridHeight = 50;
        terrainMgr.showGrid = true;
        
        // Add Map Manager for spawn points and map info
        var mapMgr = terrainParent.AddComponent<MapManager>();
        mapMgr.mapName = "Default Map";
        mapMgr.maxPlayers = 4;
        mapMgr.mapSize = new Vector2(100, 100);
        
        // Add Building Placer system
        var buildingPlacer = gameManager.AddComponent<BuildingPlacer>();
        buildingPlacer.terrainManager = terrainMgr;
        // Note: buildingPrefab should be assigned in the inspector or via code
        
        // Create back to menu button
        var backBtn = MakeButton(gameCanvas.transform, "BackToMenuButton", "Main Menu");
        var backBtnRect = backBtn.GetComponent<RectTransform>();
        backBtnRect.anchorMin = new Vector2(0, 1);
        backBtnRect.anchorMax = new Vector2(0, 1);
        backBtnRect.pivot = new Vector2(0, 1);
        backBtnRect.anchoredPosition = new Vector2(20, -20);
        backBtnRect.sizeDelta = new Vector2(200, 80);
        backBtn.onClick.AddListener(() => gameMgr.ReturnToMainMenu());
        var gamePath = "Assets/Scenes/Game.unity";
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), gamePath);

        // Reopen MainMenu
        EditorSceneManager.OpenScene(mmPath);

        // Add both to Build Settings
        var gameModeSelectionPath = "Assets/Scenes/GameModeSelection.unity";
        CreateGameModeSelectionScene(gameModeSelectionPath);
        
        var list = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(mmPath, true),
            new EditorBuildSettingsScene(gameModeSelectionPath, true),
            new EditorBuildSettingsScene(gamePath, true)
        };
        EditorBuildSettings.scenes = list;

    // Ensure any existing EventSystems in loaded scenes use the Input System module
    UpgradeEventSystemsInOpenScenes();

    EditorUtility.DisplayDialog("Main Menu", "Main Menu + Game Mode Selection + Game scenes created, upgraded, and added to Build Settings.", "OK");

        // ------- local UI builders -------
        Dropdown MakeLabeledDropdown(Transform parent, string label, Vector2 offset)
        {
            var root = new GameObject(label, typeof(RectTransform));
            root.transform.SetParent(parent, false);
            var r = root.GetComponent<RectTransform>();
            r.sizeDelta = new Vector2(700, 100);
            r.anchoredPosition = offset;

            var lab = new GameObject("Label", typeof(Text));
            lab.transform.SetParent(root.transform, false);
            var lt = lab.GetComponent<Text>();
            lt.text = label;
            lt.font = GetDefaultFont();
            lt.color = Color.white;
            lt.alignment = TextAnchor.MiddleLeft;
            var lr = lab.GetComponent<RectTransform>();
            lr.anchorMin = new Vector2(0, 0.5f); lr.anchorMax = new Vector2(0, 0.5f);
            lr.anchoredPosition = new Vector2(-220, 0);
            lr.sizeDelta = new Vector2(360, 80);

            var dd = new GameObject("Dropdown", typeof(Image), typeof(Dropdown));
            dd.transform.SetParent(root.transform, false);
            var dropdown = dd.GetComponent<Dropdown>();
            var ddImg = dd.GetComponent<Image>();
            ddImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            var ddr = dd.GetComponent<RectTransform>();
            ddr.sizeDelta = new Vector2(380, 80);
            ddr.anchoredPosition = new Vector2(160, 0);

            // Add some basic options
            dropdown.options.Add(new Dropdown.OptionData("Low"));
            dropdown.options.Add(new Dropdown.OptionData("Medium"));
            dropdown.options.Add(new Dropdown.OptionData("High"));
            dropdown.options.Add(new Dropdown.OptionData("Ultra"));
            dropdown.value = 2; // Default to High

            return dropdown;
        }

        Slider MakeLabeledSlider(Transform parent, string label, Vector2 offset, float min, float max, float value)
        {
            var root = new GameObject(label, typeof(RectTransform));
            root.transform.SetParent(parent, false);
            var r = root.GetComponent<RectTransform>();
            r.sizeDelta = new Vector2(700, 100);
            r.anchoredPosition = offset;

            var lab = new GameObject("Label", typeof(Text));
            lab.transform.SetParent(root.transform, false);
            var lt = lab.GetComponent<Text>();
            lt.text = label;
            lt.font = GetDefaultFont();
            lt.color = Color.white;
            lt.alignment = TextAnchor.MiddleLeft;
            var lr = lab.GetComponent<RectTransform>();
            lr.anchorMin = new Vector2(0, 0.5f); lr.anchorMax = new Vector2(0, 0.5f);
            lr.anchoredPosition = new Vector2(-220, 0);
            lr.sizeDelta = new Vector2(360, 80);

            var sl = new GameObject("Slider", typeof(Slider));
            sl.transform.SetParent(root.transform, false);
            var s = sl.GetComponent<Slider>();
            s.minValue = min; s.maxValue = max; s.value = value;
            var sr = sl.GetComponent<RectTransform>();
            sr.sizeDelta = new Vector2(380, 80);
            sr.anchoredPosition = new Vector2(160, 0);

            // Create slider background
            var bg = new GameObject("Background", typeof(Image));
            bg.transform.SetParent(sl.transform, false);
            bg.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            var bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero; bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero; bgRect.offsetMax = Vector2.zero;

            // Create fill area and handle (simplified)
            var handle = new GameObject("Handle", typeof(Image));
            handle.transform.SetParent(sl.transform, false);
            handle.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
            var handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 80);
            s.targetGraphic = handle.GetComponent<Image>();

            return s;
        }

        Toggle MakeLabeledToggle(Transform parent, string label, Vector2 offset, bool value)
        {
            var root = new GameObject(label, typeof(RectTransform));
            root.transform.SetParent(parent, false);
            var r = root.GetComponent<RectTransform>();
            r.sizeDelta = new Vector2(700, 100);
            r.anchoredPosition = offset;

            var lab = new GameObject("Label", typeof(Text));
            lab.transform.SetParent(root.transform, false);
            var lt = lab.GetComponent<Text>();
            lt.text = label;
            lt.font = GetDefaultFont();
            lt.color = Color.white;
            lt.alignment = TextAnchor.MiddleLeft;
            var lr = lab.GetComponent<RectTransform>();
            lr.anchorMin = new Vector2(0, 0.5f); lr.anchorMax = new Vector2(0, 0.5f);
            lr.anchoredPosition = new Vector2(-220, 0);
            lr.sizeDelta = new Vector2(360, 80);

            var tg = new GameObject("Toggle", typeof(Toggle), typeof(Image));
            tg.transform.SetParent(root.transform, false);
            var t = tg.GetComponent<Toggle>();
            var tgImg = tg.GetComponent<Image>();
            tgImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            t.isOn = value;
            var tr = tg.GetComponent<RectTransform>();
            tr.sizeDelta = new Vector2(80, 80);
            tr.anchoredPosition = new Vector2(160, 0);

            // Create checkmark
            var checkmark = new GameObject("Checkmark", typeof(Image));
            checkmark.transform.SetParent(tg.transform, false);
            var checkImg = checkmark.GetComponent<Image>();
            checkImg.color = new Color(0.2f, 0.8f, 0.2f, 1f);
            var checkRect = checkmark.GetComponent<RectTransform>();
            checkRect.anchorMin = Vector2.zero; checkRect.anchorMax = Vector2.one;
            checkRect.offsetMin = new Vector2(10, 10); checkRect.offsetMax = new Vector2(-10, -10);
            t.graphic = checkImg;

            return t;
        }
        
        // Create RTS-style terrain with grid visualization
        void CreateRTSTerrain(Transform parent)
        {
            // Create base terrain plane
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(parent);
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(50, 1, 50); // 500x500 units
            
            // Create simple grass-like material
            var terrainMaterial = new Material(Shader.Find("Standard"));
            terrainMaterial.color = new Color(0.3f, 0.5f, 0.2f, 1f); // Grass green
            terrainMaterial.SetFloat("_Metallic", 0f);
            terrainMaterial.SetFloat("_Glossiness", 0.1f); // Rough terrain surface
            ground.GetComponent<Renderer>().material = terrainMaterial;
            
            // Create grid lines using LineRenderer components for clear visibility
            CreateGridLines(parent, 50, 50, 2f); // 50x50 grid with 2 unit spacing
            
            // Add some terrain variation with small hills
            CreateTerrainFeatures(parent);
        }
        
        // Create visible grid lines for building placement
        void CreateGridLines(Transform parent, int width, int height, float spacing)
        {
            var gridParent = new GameObject("GridLines");
            gridParent.transform.SetParent(parent);
            
            // Create material for grid lines
            // Use an SRP/URP-friendly default shader for lines
            Shader lineShader = Shader.Find("Universal Render Pipeline/Simple Lit");
            if (lineShader == null) lineShader = Shader.Find("Sprites/Default");
            var lineMaterial = new Material(lineShader);
            lineMaterial.color = new Color(0.5f, 0.7f, 0.3f, 0.8f); // Subtle green lines
            
            // Vertical lines
            for (int x = 0; x <= width; x++)
            {
                var line = new GameObject($"GridLine_V_{x}");
                line.transform.SetParent(gridParent.transform);
                var lr = line.AddComponent<LineRenderer>();
                lr.material = lineMaterial;
                lr.startWidth = 0.1f;
                lr.endWidth = 0.1f;
                lr.positionCount = 2;
                
                float xPos = (x - width/2f) * spacing;
                lr.SetPosition(0, new Vector3(xPos, 0.1f, -height/2f * spacing));
                lr.SetPosition(1, new Vector3(xPos, 0.1f, height/2f * spacing));
            }
            
            // Horizontal lines
            for (int z = 0; z <= height; z++)
            {
                var line = new GameObject($"GridLine_H_{z}");
                line.transform.SetParent(gridParent.transform);
                var lr = line.AddComponent<LineRenderer>();
                lr.material = lineMaterial;
                lr.startWidth = 0.1f;
                lr.endWidth = 0.1f;
                lr.positionCount = 2;
                
                float zPos = (z - height/2f) * spacing;
                lr.SetPosition(0, new Vector3(-width/2f * spacing, 0.1f, zPos));
                lr.SetPosition(1, new Vector3(width/2f * spacing, 0.1f, zPos));
            }
        }
        
        // Add some terrain features for visual interest
        void CreateTerrainFeatures(Transform parent)
        {
            var featuresParent = new GameObject("TerrainFeatures");
            featuresParent.transform.SetParent(parent);
            
            // Create a few small hills/rocks
            for (int i = 0; i < 5; i++)
            {
                var hill = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                hill.name = $"Hill_{i}";
                hill.transform.SetParent(featuresParent.transform);
                hill.transform.position = new Vector3(
                    UnityEngine.Random.Range(-40f, 40f),
                    0.5f,
                    UnityEngine.Random.Range(-40f, 40f)
                );
                hill.transform.localScale = new Vector3(
                    UnityEngine.Random.Range(2f, 5f),
                    UnityEngine.Random.Range(0.5f, 1.5f),
                    UnityEngine.Random.Range(2f, 5f)
                );
                
                // Hill material
                var hillMaterial = new Material(Shader.Find("Standard"));
                hillMaterial.color = new Color(0.4f, 0.3f, 0.2f, 1f); // Brown/dirt color
                hill.GetComponent<Renderer>().material = hillMaterial;
            }
            
            // Create some tree placeholders
            for (int i = 0; i < 8; i++)
            {
                var tree = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                tree.name = $"Tree_{i}";
                tree.transform.SetParent(featuresParent.transform);
                tree.transform.position = new Vector3(
                    UnityEngine.Random.Range(-30f, 30f),
                    2f,
                    UnityEngine.Random.Range(-30f, 30f)
                );
                tree.transform.localScale = new Vector3(0.5f, 4f, 0.5f);
                
                // Tree material
                var treeMaterial = new Material(Shader.Find("Standard"));
                treeMaterial.color = new Color(0.2f, 0.4f, 0.1f, 1f); // Dark green
                tree.GetComponent<Renderer>().material = treeMaterial;
            }
        }
    }
    
    static void CreateGameModeSelectionScene(string scenePath)
    {
        // Create Game Mode Selection scene
        var gameModeScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        
    // Create EventSystem for UI (Input System)
    var es = new GameObject("EventSystem");
    es.AddComponent<UnityEngine.EventSystems.EventSystem>();
    es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        
        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvasComp = canvasGO.GetComponent<Canvas>();
        canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        UIDesignSystem.SetupResponsiveCanvas(scaler);
        
        // Background
        var bg = new GameObject("Background", typeof(Image));
        bg.transform.SetParent(canvasGO.transform, false);
        var bgImage = bg.GetComponent<Image>();
        bgImage.color = UIDesignSystem.Colors.PrimaryDark;
        var bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero; 
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero; 
        bgRect.offsetMax = Vector2.zero;
        
        // Header section
        var header = new GameObject("Header", typeof(RectTransform));
        header.transform.SetParent(canvasGO.transform, false);
        var headerRect = header.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0f, 0.8f);
        headerRect.anchorMax = new Vector2(1f, 1f);
        headerRect.offsetMin = Vector2.zero;
        headerRect.offsetMax = Vector2.zero;
        
        // Title
        var title = new GameObject("Title", typeof(Text));
        title.transform.SetParent(header.transform, false);
        var titleText = title.GetComponent<Text>();
        titleText.text = "⚔ SELECT GAME MODE ⚔";
        titleText.font = GetDefaultFont();
        titleText.fontSize = UIDesignSystem.Typography.TitleMedium;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = UIDesignSystem.Colors.AccentGoldLight;
        titleText.alignment = TextAnchor.MiddleCenter;
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = Vector2.zero;
        titleRect.anchorMax = Vector2.one;
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(canvasGO.transform, false);
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.2f, 0.2f);
        contentRect.anchorMax = new Vector2(0.8f, 0.75f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        
        var contentVlg = content.GetComponent<VerticalLayoutGroup>();
        contentVlg.spacing = UIDesignSystem.Spacing.XLarge;
        contentVlg.childAlignment = TextAnchor.MiddleCenter;
        contentVlg.childControlWidth = false;
        contentVlg.childControlHeight = false;
        contentVlg.padding = new RectOffset(0, 0, (int)UIDesignSystem.Spacing.XLarge, 0);
        
        // Create game mode buttons with descriptions
        var campaignBtn = UIDesignSystem.ButtonFactory.CreateGameModeButton(content.transform, "CampaignButton", "🏰 CAMPAIGN", "Embark on epic single-player missions", UIDesignSystem.Colors.ButtonPrimary);
        var skirmishBtn = UIDesignSystem.ButtonFactory.CreateGameModeButton(content.transform, "SkirmishButton", "⚔ SKIRMISH", "Battle against AI opponents", UIDesignSystem.Colors.ButtonSuccess);
        var multiplayerBtn = UIDesignSystem.ButtonFactory.CreateGameModeButton(content.transform, "MultiplayerButton", "🌐 MULTIPLAYER", "Fight other players online", UIDesignSystem.Colors.ButtonSecondary);
        
        // Back button in bottom area
        var bottomArea = new GameObject("BottomArea", typeof(RectTransform));
        bottomArea.transform.SetParent(canvasGO.transform, false);
        var bottomRect = bottomArea.GetComponent<RectTransform>();
        bottomRect.anchorMin = new Vector2(0f, 0f);
        bottomRect.anchorMax = new Vector2(1f, 0.15f);
        bottomRect.offsetMin = Vector2.zero;
        bottomRect.offsetMax = Vector2.zero;
        
        var backBtn = UIDesignSystem.ButtonFactory.CreateSecondaryButton(bottomArea.transform, "BackButton", "← BACK TO MAIN MENU");
        var backBtnRect = backBtn.GetComponent<RectTransform>();
        backBtnRect.anchorMin = new Vector2(0.1f, 0.3f);
        backBtnRect.anchorMax = new Vector2(0.4f, 0.7f);
        backBtnRect.offsetMin = Vector2.zero;
        backBtnRect.offsetMax = Vector2.zero;
        
        // Create panels for each mode (initially hidden)
        var campaignPanel = CreatePanel(canvasGO.transform, "CampaignPanel", false);
        var skirmishPanel = CreatePanel(canvasGO.transform, "SkirmishPanel", false);
        var multiplayerPanel = CreatePanel(canvasGO.transform, "MultiplayerPanel", false);
        
        // Add Campaign UI to panel
        CreateCampaignUI(campaignPanel.transform);
        
        // Add Skirmish UI to panel
        CreateSkirmishUI(skirmishPanel.transform);
        
        // Add GameModeMenu component
        var gameModeMenu = canvasGO.AddComponent<GameModeMenu>();
        gameModeMenu.campaignButton = campaignBtn;
        gameModeMenu.skirmishButton = skirmishBtn;
        gameModeMenu.multiplayerButton = multiplayerBtn;
        gameModeMenu.backToMainButton = backBtn;
        gameModeMenu.campaignPanel = campaignPanel;
        gameModeMenu.skirmishPanel = skirmishPanel;
        gameModeMenu.multiplayerPanel = multiplayerPanel;
        
        // Save scene
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), scenePath);
    }
    
    static GameObject CreatePanel(Transform parent, string name, bool active)
    {
        var panel = new GameObject(name, typeof(Image));
        panel.transform.SetParent(parent, false);
        var image = panel.GetComponent<Image>();
        image.color = new Color(0, 0, 0, 0.8f);
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
        panel.SetActive(active);
        return panel;
    }
    
    static void SetButtonPosition(Button button, Vector2 anchorPos, Vector2 size)
    {
        var rect = button.GetComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
    }
    
    static void CreateCampaignUI(Transform parent)
    {
        var title = new GameObject("CampaignTitle", typeof(Text));
        title.transform.SetParent(parent, false);
        var titleText = title.GetComponent<Text>();
        titleText.text = "CAMPAIGN MODE";
    titleText.font = GetDefaultFont();
        titleText.fontSize = 48;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.7f);
        titleRect.anchorMax = new Vector2(1, 0.9f);
        titleRect.offsetMin = Vector2.zero; titleRect.offsetMax = Vector2.zero;
        
        var newBtn = MakeButton(parent, "NewCampaignButton", "NEW CAMPAIGN");
        SetButtonPosition(newBtn, new Vector2(0.5f, 0.6f), new Vector2(300, 60));
        
        var loadBtn = MakeButton(parent, "LoadCampaignButton", "LOAD CAMPAIGN");
        SetButtonPosition(loadBtn, new Vector2(0.5f, 0.5f), new Vector2(300, 60));
        
        var backBtn = MakeButton(parent, "CampaignBackButton", "BACK");
        SetButtonPosition(backBtn, new Vector2(0.5f, 0.3f), new Vector2(200, 50));
        
        // Wire up back button to return to main game mode selection - fix component reference
        backBtn.onClick.AddListener(() => {
            // Find the GameModeMenu component properly
            var canvas = parent.root.gameObject;
            var gameModeMenu = canvas.GetComponent<GameModeMenu>();
            if (gameModeMenu != null) gameModeMenu.ShowMainGameModeSelection();
        });
        
        // Wire up buttons (this would be done in GameModeMenu)
        var gameModeMenu = parent.GetComponentInParent<GameModeMenu>();
        if (gameModeMenu != null)
        {
            gameModeMenu.newCampaignButton = newBtn;
            gameModeMenu.loadCampaignButton = loadBtn;
        }
    }
    
    static void CreateSkirmishUI(Transform parent)
    {
        var scrollGO = new GameObject("SkirmishScrollView", typeof(RectTransform), typeof(ScrollRect), typeof(Image));
        scrollGO.transform.SetParent(parent, false);
        var scrollRect = scrollGO.GetComponent<ScrollRect>();
        var scrollImg = scrollGO.GetComponent<Image>();
        scrollImg.color = UIDesignSystem.Colors.SecondaryDark;
        
        var scrollRectTransform = scrollGO.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = new Vector2(0f, 0.15f);
        scrollRectTransform.anchorMax = new Vector2(1f, 1f);
        scrollRectTransform.offsetMin = Vector2.zero;
        scrollRectTransform.offsetMax = Vector2.zero;

        // Viewport
        var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
        viewport.transform.SetParent(scrollGO.transform, false);
        var viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewport.GetComponent<Image>().color = Color.clear;

        // Content container
        var container = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        container.transform.SetParent(viewport.transform, false);
        var containerRect = container.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0f, 1f);
        containerRect.anchorMax = new Vector2(1f, 1f);
        containerRect.pivot = new Vector2(0.5f, 1f);
        containerRect.anchoredPosition = Vector2.zero;

        var contentVlg = container.GetComponent<VerticalLayoutGroup>();
        contentVlg.spacing = UIDesignSystem.Spacing.Large;
        contentVlg.padding = new RectOffset(
            (int)UIDesignSystem.Spacing.Large, 
            (int)UIDesignSystem.Spacing.Large, 
            (int)UIDesignSystem.Spacing.Large, 
            (int)UIDesignSystem.Spacing.Large
        );
        contentVlg.childControlWidth = true;
        contentVlg.childControlHeight = false;
        contentVlg.childForceExpandWidth = true;

        var contentFitter = container.GetComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewportRect;
        scrollRect.content = containerRect;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        CreateSkirmishTitle(container.transform);
        
        CreateMobileMapSelection(container.transform);
        
        // Player setup section
        CreateMobilePlayerSetup(container.transform);

        CreateSkirmishActionBar(parent);

        // Hook to SkirmishSetup
        var skirmishSetup = scrollGO.AddComponent<SkirmishSetup>();
    }
    
    static void CreateMapSelectionSection(Transform parent)
    {
        var section = new GameObject("MapSection", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(Image));
        section.transform.SetParent(parent, false);
        
        var sectionImg = section.GetComponent<Image>();
        sectionImg.color = new Color(0.12f, 0.10f, 0.08f, 0.9f);
        
        var sectionHlg = section.GetComponent<HorizontalLayoutGroup>();
        sectionHlg.spacing = 20f;
        sectionHlg.padding = new RectOffset(20, 20, 15, 15);
        sectionHlg.childControlWidth = false;
        sectionHlg.childControlHeight = true;
        sectionHlg.childForceExpandWidth = false;
        sectionHlg.childForceExpandHeight = false;
        
        var sectionLayout = section.AddComponent<LayoutElement>();
        sectionLayout.preferredHeight = 80;
        
        // Map label
        var mapLabel = CreateMilitaryLabel(section.transform, "Map:", 100);
        var mapLabelLayout = mapLabel.AddComponent<LayoutElement>();
        mapLabelLayout.preferredWidth = 100;
        
        // Map dropdown
        var mapDropdown = CreateMilitaryDropdown(section.transform, new[] { 
            "Valley Forge (4 Players)", 
            "Highland Clash (4 Players)", 
            "Desert Storm (4 Players)" 
        });
        var mapDropdownLayout = mapDropdown.gameObject.AddComponent<LayoutElement>();
        mapDropdownLayout.flexibleWidth = 1f;
        mapDropdownLayout.preferredHeight = 50;
    }
    
    static void CreatePlayerTableHeader(Transform parent)
    {
        var header = new GameObject("PlayerTableHeader", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(Image));
        header.transform.SetParent(parent, false);
        
        var headerImg = header.GetComponent<Image>();
        headerImg.color = new Color(1f, 0.85f, 0.4f, 0.3f); // Gold header
        
        var headerHlg = header.GetComponent<HorizontalLayoutGroup>();
        headerHlg.spacing = 5f;
        headerHlg.padding = new RectOffset(10, 10, 8, 8);
        headerHlg.childControlWidth = false;
        headerHlg.childControlHeight = true;
        headerHlg.childForceExpandWidth = false;
        headerHlg.childForceExpandHeight = false;
        
        var headerLayout = header.AddComponent<LayoutElement>();
        headerLayout.preferredHeight = 40;
        
        // Header labels with proper widths for mobile
        CreateHeaderLabel(header.transform, "Players", 120);
        CreateHeaderLabel(header.transform, "Difficulty", 100);
        CreateHeaderLabel(header.transform, "Wood", 80);
        CreateHeaderLabel(header.transform, "Stone", 80);
        CreateHeaderLabel(header.transform, "Iron", 80);
        CreateHeaderLabel(header.transform, "Gold", 80);
        CreateHeaderLabel(header.transform, "Team", 100);
        CreateHeaderLabel(header.transform, "Action", 80);
    }
    
    static void CreateHeaderLabel(Transform parent, string text, float width)
    {
        var label = new GameObject($"Header_{text}", typeof(Text));
        label.transform.SetParent(parent, false);
        var labelText = label.GetComponent<Text>();
        labelText.text = text;
        labelText.font = GetDefaultFont();
        labelText.fontSize = 16;
        labelText.fontStyle = FontStyle.Bold;
        labelText.color = new Color(0.1f, 0.1f, 0.1f, 1f); // Dark text on gold
        labelText.alignment = TextAnchor.MiddleCenter;
        
        var layout = label.AddComponent<LayoutElement>();
        layout.preferredWidth = width;
    }
    
    static void CreateMobilePlayerRow(Transform parent, string playerName, string difficulty, int wood, int stone, int iron, int gold, string team, bool canRemove)
    {
        var row = new GameObject($"PlayerRow_{playerName}", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(Image));
        row.transform.SetParent(parent, false);
        
        var rowImg = row.GetComponent<Image>();
        rowImg.color = new Color(0.08f, 0.12f, 0.16f, 0.6f);
        
        var hlg = row.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 5f;
        hlg.padding = new RectOffset(10, 10, 8, 8);
        hlg.childControlWidth = false;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
        
        var rowLayout = row.AddComponent<LayoutElement>();
        rowLayout.preferredHeight = 50;
        
        // Player name
        CreateRowLabel(row.transform, playerName, 120);
        
        // Difficulty dropdown or label
        if (playerName == "Player")
        {
            CreateRowLabel(row.transform, "Human", 100);
        }
        else
        {
            var diffDropdown = CreateCompactMilitaryDropdown(row.transform, new[] { "Easy", "Medium", "Hard", "Expert" }, 100);
            diffDropdown.value = difficulty == "Easy" ? 0 : difficulty == "Medium" ? 1 : difficulty == "Hard" ? 2 : 3;
        }
        
        // Resource fields
        CreateResourceField(row.transform, wood.ToString(), 80);
        CreateResourceField(row.transform, stone.ToString(), 80);
        CreateResourceField(row.transform, iron.ToString(), 80);
        CreateResourceField(row.transform, gold.ToString(), 80);
        
        // Team dropdown
        var teamDropdown = CreateCompactMilitaryDropdown(row.transform, new[] { "None", "Team1", "Team2", "Team3" }, 100);
        teamDropdown.value = team == "None" ? 0 : team == "Team1" ? 1 : team == "Team2" ? 2 : 3;
        
        // Remove button or blank
        if (canRemove)
        {
            var removeBtn = CreateSmallMilitaryButton(row.transform, "Remove", new Color(0.6f, 0.2f, 0.1f, 1f), 80);
        }
        else
        {
            CreateRowLabel(row.transform, "", 80); // Empty space
        }
    }
    
    static void CreateRowLabel(Transform parent, string text, float width)
    {
        var label = new GameObject($"RowLabel_{text}", typeof(Text));
        label.transform.SetParent(parent, false);
        var labelText = label.GetComponent<Text>();
        labelText.text = text;
        labelText.font = GetDefaultFont();
        labelText.fontSize = 14;
        labelText.color = new Color(1f, 0.95f, 0.8f, 1f);
        labelText.alignment = TextAnchor.MiddleCenter;
        
        var layout = label.AddComponent<LayoutElement>();
        layout.preferredWidth = width;
    }
    
    static void CreateResourceField(Transform parent, string value, float width)
    {
        var field = new GameObject("ResourceField", typeof(Image), typeof(InputField));
        field.transform.SetParent(parent, false);
        
        var fieldImg = field.GetComponent<Image>();
        fieldImg.color = new Color(0.15f, 0.12f, 0.08f, 1f);
        
        var inputField = field.GetComponent<InputField>();
        inputField.text = value;
        
        // Text component for the input field
        var textObj = new GameObject("Text", typeof(Text));
        textObj.transform.SetParent(field.transform, false);
        var text = textObj.GetComponent<Text>();
        text.font = GetDefaultFont();
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(5, 2);
        textRect.offsetMax = new Vector2(-5, -2);
        
        inputField.textComponent = text;
        
        var layout = field.AddComponent<LayoutElement>();
        layout.preferredWidth = width;
    }
    
    static Button CreateSmallMilitaryButton(Transform parent, string text, Color color, float width)
    {
        var btn = new GameObject($"SmallButton_{text}", typeof(Image), typeof(Button));
        btn.transform.SetParent(parent, false);
        
        var img = btn.GetComponent<Image>();
        img.color = color;
        
        // Text
        var textObj = new GameObject("Text", typeof(Text));
        textObj.transform.SetParent(btn.transform, false);
        var textComp = textObj.GetComponent<Text>();
        textComp.text = text;
        textComp.font = GetDefaultFont();
        textComp.fontSize = 12;
        textComp.fontStyle = FontStyle.Bold;
        textComp.color = Color.white;
        textComp.alignment = TextAnchor.MiddleCenter;
        
        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        var layout = btn.AddComponent<LayoutElement>();
        layout.preferredWidth = width;
        
        return btn.GetComponent<Button>();
    }
    
    static void CreateSection(Transform parent, string title, System.Action<Transform> createContent)
    {
        var section = new GameObject($"Section_{title.Replace(" ", "")}", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter), typeof(Image));
        section.transform.SetParent(parent, false);
        
        var sectionImg = section.GetComponent<Image>();
        sectionImg.color = new Color(0.12f, 0.10f, 0.08f, 0.9f); // Dark military with transparency
        
        var sectionVlg = section.GetComponent<VerticalLayoutGroup>();
        sectionVlg.spacing = 15f;
        sectionVlg.padding = new RectOffset(25, 25, 20, 20);
        sectionVlg.childControlWidth = true;
        sectionVlg.childControlHeight = false;
        sectionVlg.childForceExpandWidth = true;
        
        var sectionFitter = section.GetComponent<ContentSizeFitter>();
        sectionFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // Add subtle border
        var border = new GameObject("Border", typeof(Image));
        border.transform.SetParent(section.transform, false);
        var borderImg = border.GetComponent<Image>();
        borderImg.color = new Color(1f, 0.85f, 0.4f, 0.3f); // Gold border
        var borderRect = border.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero; borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(0, 0); borderRect.offsetMax = new Vector2(0, 0);
        
        var innerBorder = new GameObject("InnerBorder", typeof(Image));
        innerBorder.transform.SetParent(border.transform, false);
        var innerImg = innerBorder.GetComponent<Image>();
        innerImg.color = new Color(0.12f, 0.10f, 0.08f, 1f);
        var innerRect = innerBorder.GetComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero; innerRect.anchorMax = Vector2.one;
        innerRect.offsetMin = new Vector2(3, 3); innerRect.offsetMax = new Vector2(-3, -3);
        
        // Section title with military styling
        var titleObj = new GameObject("Title", typeof(Text));
        titleObj.transform.SetParent(section.transform, false);
        var titleText = titleObj.GetComponent<Text>();
        titleText.text = title;
        titleText.font = GetDefaultFont();
        titleText.fontSize = 32;
        titleText.color = new Color(1f, 0.85f, 0.4f, 1f); // Gold
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        
        var titleLayout = titleObj.AddComponent<LayoutElement>();
        titleLayout.preferredHeight = 45;
        
        createContent(section.transform);
    }
    
    static void CreateMobilePlayerRow(Transform parent, int playerIndex)
    {
        var row = new GameObject($"Player_{playerIndex}", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(Image));
        row.transform.SetParent(parent, false);
        
        var rowImg = row.GetComponent<Image>();
        rowImg.color = new Color(0.08f, 0.12f, 0.16f, 0.7f); // Dark blue-gray
        
        var vlg = row.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = 8f;
        vlg.padding = new RectOffset(15, 15, 12, 12);
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        
        var rowLayout = row.AddComponent<LayoutElement>();
        rowLayout.preferredHeight = 120;
        
        // Player name row
        var nameRow = new GameObject("NameRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        nameRow.transform.SetParent(row.transform, false);
        var nameHlg = nameRow.GetComponent<HorizontalLayoutGroup>();
        nameHlg.spacing = 15f;
        nameHlg.childControlWidth = false;
        nameHlg.childControlHeight = true;
        nameHlg.childForceExpandWidth = false;
        
        var nameLayout = nameRow.AddComponent<LayoutElement>();
        nameLayout.preferredHeight = 40;
        
        // Player label with rank
        var label = CreateMilitaryLabel(nameRow.transform, $"⭐ PLAYER {playerIndex + 1}", 150);
        
        // Color selector button
        var colorBtn = CreateMobileColorButton(nameRow.transform, GetPlayerColor(playerIndex));
        
        // Controls row
        var controlsRow = new GameObject("ControlsRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        controlsRow.transform.SetParent(row.transform, false);
        var controlsHlg = controlsRow.GetComponent<HorizontalLayoutGroup>();
        controlsHlg.spacing = 10f;
        controlsHlg.childControlWidth = false;
        controlsHlg.childControlHeight = true;
        controlsHlg.childForceExpandWidth = false;
        
        var controlsLayout = controlsRow.AddComponent<LayoutElement>();
        controlsLayout.preferredHeight = 50;
        
        // Player type dropdown
        var typeDropdown = CreateCompactMilitaryDropdown(controlsRow.transform, new[] { "Human", "AI Easy", "AI Normal", "AI Hard", "Disabled" }, 140);
        
        // Team dropdown  
        var teamDropdown = CreateCompactMilitaryDropdown(controlsRow.transform, new[] { "No Team", "Team 1", "Team 2", "Team 3" }, 120);
    }
    
    static void CreateResourceButton(Transform parent, string icon, string resourceName, string amount)
    {
        var btn = new GameObject($"{resourceName}Button", typeof(Image), typeof(Button));
        btn.transform.SetParent(parent, false);
        var img = btn.GetComponent<Image>();
        img.color = new Color(0.2f, 0.15f, 0.1f, 1f); // Dark military brown
        
        // Icon
        var iconObj = new GameObject("Icon", typeof(Text));
        iconObj.transform.SetParent(btn.transform, false);
        var iconText = iconObj.GetComponent<Text>();
        iconText.text = icon;
        iconText.font = GetDefaultFont();
        iconText.fontSize = 32;
        iconText.alignment = TextAnchor.MiddleCenter;
        var iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.5f);
        iconRect.anchorMax = new Vector2(1, 1f);
        iconRect.offsetMin = Vector2.zero; iconRect.offsetMax = Vector2.zero;
        
        // Resource name
        var nameObj = new GameObject("Name", typeof(Text));
        nameObj.transform.SetParent(btn.transform, false);
        var nameText = nameObj.GetComponent<Text>();
        nameText.text = resourceName;
        nameText.font = GetDefaultFont();
        nameText.fontSize = 16;
        nameText.fontStyle = FontStyle.Bold;
        nameText.color = new Color(1f, 0.85f, 0.4f, 1f); // Gold
        nameText.alignment = TextAnchor.MiddleCenter;
        var nameRect = nameObj.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.3f);
        nameRect.anchorMax = new Vector2(1, 0.5f);
        nameRect.offsetMin = Vector2.zero; nameRect.offsetMax = Vector2.zero;
        
        // Amount
        var amountObj = new GameObject("Amount", typeof(Text));
        amountObj.transform.SetParent(btn.transform, false);
        var amountText = amountObj.GetComponent<Text>();
        amountText.text = amount;
        amountText.font = GetDefaultFont();
        amountText.fontSize = 20;
        amountText.fontStyle = FontStyle.Bold;
        amountText.color = Color.white;
        amountText.alignment = TextAnchor.MiddleCenter;
        var amountRect = amountObj.GetComponent<RectTransform>();
        amountRect.anchorMin = new Vector2(0, 0f);
        amountRect.anchorMax = new Vector2(1, 0.3f);
        amountRect.offsetMin = Vector2.zero; amountRect.offsetMax = Vector2.zero;
        
        // Gold border
        var border = new GameObject("Border", typeof(Image));
        border.transform.SetParent(btn.transform, false);
        var borderImg = border.GetComponent<Image>();
        borderImg.color = new Color(1f, 0.85f, 0.4f, 0.6f);
        var borderRect = border.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero; borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(2, 2); borderRect.offsetMax = new Vector2(-2, -2);
    }
    
    static void CreatePlayerRow(Transform parent, int playerIndex)
    {
        var row = new GameObject($"Player_{playerIndex}", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(Image));
        row.transform.SetParent(parent, false);
        
        var rowImg = row.GetComponent<Image>();
        rowImg.color = new Color(0.08f, 0.12f, 0.16f, 0.6f);
        
        var hlg = row.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 15f;
        hlg.padding = new RectOffset(15, 15, 10, 10);
        hlg.childControlWidth = false;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
        
        var rowLayout = row.AddComponent<LayoutElement>();
        rowLayout.preferredHeight = 50;
        
        // Player label
        var label = CreateLabel(row.transform, $"Player {playerIndex + 1}", 120);
        
        // Player type dropdown
        var typeDropdown = CreateCompactDropdown(row.transform, new[] { "Human", "AI Easy", "AI Normal", "AI Hard", "Disabled" }, 120);
        
        // Team dropdown
        var teamDropdown = CreateCompactDropdown(row.transform, new[] { "Team 1", "Team 2", "Team 3", "Team 4" }, 100);
        
        // Color indicator
        var colorBtn = CreateColorButton(row.transform, GetPlayerColor(playerIndex));
        
        // Starting resources (compact)
        CreateResourceMini(row.transform, "Gold", "1000");
    }
    
    static Color GetPlayerColor(int index)
    {
        Color[] colors = {
            new Color(0.2f, 0.6f, 1f, 1f),    // Blue
            new Color(1f, 0.3f, 0.3f, 1f),    // Red  
            new Color(0.3f, 0.8f, 0.3f, 1f),  // Green
            new Color(1f, 0.8f, 0.2f, 1f),    // Yellow
            new Color(0.8f, 0.3f, 0.8f, 1f),  // Purple
            new Color(1f, 0.5f, 0.1f, 1f),    // Orange
        };
        return colors[index % colors.Length];
    }
    
    static GameObject CreateLabel(Transform parent, string text, float width)
    {
        var label = new GameObject("Label", typeof(Text));
        label.transform.SetParent(parent, false);
        var labelText = label.GetComponent<Text>();
        labelText.text = text;
        labelText.font = GetDefaultFont();
        labelText.fontSize = 18;
        labelText.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        labelText.alignment = TextAnchor.MiddleLeft;
        
        var layout = label.AddComponent<LayoutElement>();
        layout.preferredWidth = width;
        
        return label;
    }
    
    static Button CreateColorButton(Transform parent, Color color)
    {
        var btn = new GameObject("ColorButton", typeof(Image), typeof(Button));
        btn.transform.SetParent(parent, false);
        var img = btn.GetComponent<Image>();
        img.color = color;
        
        var layout = btn.AddComponent<LayoutElement>();
        layout.preferredWidth = 40;
        
        return btn.GetComponent<Button>();
    }
    
    static void CreateResourceMini(Transform parent, string resourceName, string amount)
    {
        var container = new GameObject($"{resourceName}Container", typeof(RectTransform));
        container.transform.SetParent(parent, false);
        
        var layout = container.AddComponent<LayoutElement>();
        layout.preferredWidth = 80;
        
        var text = new GameObject("Text", typeof(Text));
        text.transform.SetParent(container.transform, false);
        var textComp = text.GetComponent<Text>();
        textComp.text = $"{resourceName}\\n{amount}";
        textComp.font = GetDefaultFont();
        textComp.fontSize = 12;
        textComp.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        textComp.alignment = TextAnchor.MiddleCenter;
        
        var textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }    // Helper methods for UI creation
    static Button MakeButton(Transform parent, string name, string label)
    {
        var go = new GameObject(name, typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.color = new Color(0.15f, 0.22f, 0.36f, 1f);
        var rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(600, 140);

        var textGO = new GameObject("Label", typeof(Text));
        textGO.transform.SetParent(go.transform, false);
        var t = textGO.GetComponent<Text>();
        t.text = label;
        t.alignment = TextAnchor.MiddleCenter;
        t.font = GetDefaultFont();
        t.color = new Color(1f, 1f, 1f, 0.97f);
        t.fontSize = 42;
        var tr = textGO.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;

        return go.GetComponent<Button>();
    }
    
    static Button CreateModernButton(Transform parent, string name, string label, Color color)
    {
        var go = new GameObject(name, typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.color = color;
        
        // Add subtle gradient effect
        var gradientObj = new GameObject("Gradient", typeof(Image));
        gradientObj.transform.SetParent(go.transform, false);
        var gradientImg = gradientObj.GetComponent<Image>();
        gradientImg.color = new Color(1f, 1f, 1f, 0.1f);
        var gradientRect = gradientObj.GetComponent<RectTransform>();
        gradientRect.anchorMin = Vector2.zero;
        gradientRect.anchorMax = new Vector2(1, 0.5f);
        gradientRect.offsetMin = Vector2.zero;
        gradientRect.offsetMax = Vector2.zero;

        var textGO = new GameObject("Label", typeof(Text));
        textGO.transform.SetParent(go.transform, false);
        var t = textGO.GetComponent<Text>();
        t.text = label;
        t.alignment = TextAnchor.MiddleCenter;
        t.font = GetDefaultFont();
        t.color = Color.white;
        t.fontSize = 24;
        t.fontStyle = FontStyle.Bold;
        var tr = textGO.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;

        return go.GetComponent<Button>();
    }
    
    static Dropdown CreateStyledDropdown(Transform parent, string label, string[] options)
    {
        var container = new GameObject($"{label}Container", typeof(RectTransform));
        container.transform.SetParent(parent, false);
        
        var layout = container.AddComponent<LayoutElement>();
        layout.preferredHeight = 60;
        
        // Label
        var labelObj = new GameObject("Label", typeof(Text));
        labelObj.transform.SetParent(container.transform, false);
        var labelText = labelObj.GetComponent<Text>();
        labelText.text = label;
        labelText.font = GetDefaultFont();
        labelText.fontSize = 18;
        labelText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        labelText.alignment = TextAnchor.MiddleLeft;
        
        var labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.6f);
        labelRect.anchorMax = new Vector2(1, 1f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        
        // Dropdown
        var dd = new GameObject("Dropdown", typeof(Image), typeof(Dropdown));
        dd.transform.SetParent(container.transform, false);
        var dropdown = dd.GetComponent<Dropdown>();
        var ddImg = dd.GetComponent<Image>();
        ddImg.color = new Color(0.15f, 0.2f, 0.25f, 1f);
        
        var ddRect = dd.GetComponent<RectTransform>();
        ddRect.anchorMin = new Vector2(0, 0f);
        ddRect.anchorMax = new Vector2(1, 0.55f);
        ddRect.offsetMin = Vector2.zero;
        ddRect.offsetMax = Vector2.zero;
        
        dropdown.options.Clear();
        foreach (var option in options)
        {
            dropdown.options.Add(new Dropdown.OptionData(option));
        }
        
        return dropdown;
    }
    
    static Dropdown CreateCompactDropdown(Transform parent, string[] options, float width)
    {
        var dd = new GameObject("CompactDropdown", typeof(Image), typeof(Dropdown));
        dd.transform.SetParent(parent, false);
        var dropdown = dd.GetComponent<Dropdown>();
        var ddImg = dd.GetComponent<Image>();
        ddImg.color = new Color(0.15f, 0.2f, 0.25f, 1f);
        
        var layout = dd.AddComponent<LayoutElement>();
        layout.preferredWidth = width;
        
        dropdown.options.Clear();
        foreach (var option in options)
        {
            dropdown.options.Add(new Dropdown.OptionData(option));
        }
        
        return dropdown;
    }
    
    static Toggle CreateStyledToggle(Transform parent, string label, bool defaultValue)
    {
        var container = new GameObject($"{label}Container", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        container.transform.SetParent(parent, false);
        
        var hlg = container.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 15f;
        hlg.childControlWidth = false;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        
        var layout = container.AddComponent<LayoutElement>();
        layout.preferredHeight = 40;
        
        // Toggle
        var toggleObj = new GameObject("Toggle", typeof(Toggle), typeof(Image));
        toggleObj.transform.SetParent(container.transform, false);
        var toggle = toggleObj.GetComponent<Toggle>();
        var toggleImg = toggleObj.GetComponent<Image>();
        toggleImg.color = new Color(0.2f, 0.25f, 0.3f, 1f);
        toggle.isOn = defaultValue;
        
        var toggleLayout = toggleObj.AddComponent<LayoutElement>();
        toggleLayout.preferredWidth = 50;
        
        // Checkmark
        var checkmark = new GameObject("Checkmark", typeof(Image));
        checkmark.transform.SetParent(toggleObj.transform, false);
        var checkImg = checkmark.GetComponent<Image>();
        checkImg.color = new Color(0.2f, 0.8f, 0.3f, 1f);
        var checkRect = checkmark.GetComponent<RectTransform>();
        checkRect.anchorMin = Vector2.zero; checkRect.anchorMax = Vector2.one;
        checkRect.offsetMin = new Vector2(5, 5); checkRect.offsetMax = new Vector2(-5, -5);
        toggle.graphic = checkImg;
        
        // Label
        var labelObj = new GameObject("Label", typeof(Text));
        labelObj.transform.SetParent(container.transform, false);
        var labelText = labelObj.GetComponent<Text>();
        labelText.text = label;
        labelText.font = GetDefaultFont();
        labelText.fontSize = 18;
        labelText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        labelText.alignment = TextAnchor.MiddleLeft;
        
        return toggle;
    }
    
    static Text CreateInfoText(Transform parent, string text)
    {
        var textObj = new GameObject("InfoText", typeof(Text));
        textObj.transform.SetParent(parent, false);
        var textComp = textObj.GetComponent<Text>();
        textComp.text = text;
        textComp.font = GetDefaultFont();
        textComp.fontSize = 14;
        textComp.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        textComp.alignment = TextAnchor.MiddleLeft;
        
        var layout = textObj.AddComponent<LayoutElement>();
        layout.preferredHeight = 30;
        
        return textComp;
    }
    
    static Button CreateMilitaryButton(Transform parent, string name, string label, Color color)
    {
        var go = new GameObject(name, typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.color = color;
        
        // Military-style border
        var border = new GameObject("Border", typeof(Image));
        border.transform.SetParent(go.transform, false);
        var borderImg = border.GetComponent<Image>();
        borderImg.color = new Color(1f, 0.85f, 0.4f, 0.8f); // Gold border
        var borderRect = border.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero; borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(0, 0); borderRect.offsetMax = new Vector2(0, 0);
        
        var innerBorder = new GameObject("InnerBorder", typeof(Image));
        innerBorder.transform.SetParent(border.transform, false);
        var innerImg = innerBorder.GetComponent<Image>();
        innerImg.color = color;
        var innerRect = innerBorder.GetComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero; innerRect.anchorMax = Vector2.one;
        innerRect.offsetMin = new Vector2(4, 4); innerRect.offsetMax = new Vector2(-4, -4);

        var textGO = new GameObject("Label", typeof(Text));
        textGO.transform.SetParent(go.transform, false);
        var t = textGO.GetComponent<Text>();
        t.text = label;
        t.alignment = TextAnchor.MiddleCenter;
        t.font = GetDefaultFont();
        t.color = new Color(1f, 0.95f, 0.8f, 1f); // Off-white
        t.fontSize = 28;
        t.fontStyle = FontStyle.Bold;
        var tr = textGO.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;

        return go.GetComponent<Button>();
    }
    
    static Dropdown CreateMilitaryDropdown(Transform parent, string[] options)
    {
        var dd = new GameObject("MilitaryDropdown", typeof(Image), typeof(Dropdown));
        dd.transform.SetParent(parent, false);
        var dropdown = dd.GetComponent<Dropdown>();
        var ddImg = dd.GetComponent<Image>();
        ddImg.color = new Color(0.15f, 0.12f, 0.08f, 1f); // Dark military brown
        
        var layout = dd.AddComponent<LayoutElement>();
        layout.preferredHeight = 60;
        layout.flexibleWidth = 1f;

    // Caption label (what shows when closed)
    var caption = new GameObject("Label", typeof(RectTransform), typeof(Text));
    caption.transform.SetParent(dd.transform, false);
    var capRect = caption.GetComponent<RectTransform>();
    capRect.anchorMin = Vector2.zero; capRect.anchorMax = Vector2.one;
    capRect.offsetMin = new Vector2(10, 5); capRect.offsetMax = new Vector2(-30, -5);
    var capText = caption.GetComponent<Text>();
    capText.font = GetDefaultFont();
    capText.fontSize = 18; capText.alignment = TextAnchor.MiddleLeft;
    capText.color = new Color(1f, 0.95f, 0.85f, 1f);
        
        // Create proper dropdown template
        var template = new GameObject("Template", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
        template.transform.SetParent(dd.transform, false);
        template.SetActive(false);
        
        var templateImg = template.GetComponent<Image>();
        templateImg.color = new Color(0.15f, 0.12f, 0.08f, 1f);
        
        var templateRect = template.GetComponent<RectTransform>();
        templateRect.anchorMin = new Vector2(0, 0);
        templateRect.anchorMax = new Vector2(1, 0);
        templateRect.anchoredPosition = new Vector2(0, 2);
        templateRect.sizeDelta = new Vector2(0, 150);
        
        // Viewport
        var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
        viewport.transform.SetParent(template.transform, false);
        var viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.sizeDelta = Vector2.zero;
        viewportRect.anchoredPosition = Vector2.zero;
        
        var viewportImg = viewport.GetComponent<Image>();
        viewportImg.color = Color.clear;
        
        // Content
        var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(viewport.transform, false);
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0, 28);
        
        var contentVlg = content.GetComponent<VerticalLayoutGroup>();
        contentVlg.spacing = 1;
        
        var contentFitter = content.GetComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // Item template
        var item = new GameObject("Item", typeof(RectTransform), typeof(Toggle), typeof(Image));
        item.transform.SetParent(content.transform, false);
        
        var itemRect = item.GetComponent<RectTransform>();
        itemRect.anchorMin = Vector2.zero;
        itemRect.anchorMax = new Vector2(1, 1);
        itemRect.sizeDelta = new Vector2(0, 20);
        
        var itemToggle = item.GetComponent<Toggle>();
        itemToggle.toggleTransition = Toggle.ToggleTransition.Fade;
        
        var itemImg = item.GetComponent<Image>();
        itemImg.color = new Color(0.15f, 0.12f, 0.08f, 1f);
        
        // Item background
        var itemBackground = new GameObject("Item Background", typeof(RectTransform), typeof(Image));
        itemBackground.transform.SetParent(item.transform, false);
        var itemBgRect = itemBackground.GetComponent<RectTransform>();
        itemBgRect.anchorMin = Vector2.zero;
        itemBgRect.anchorMax = Vector2.one;
        itemBgRect.sizeDelta = Vector2.zero;
        
        var itemBgImg = itemBackground.GetComponent<Image>();
        itemBgImg.color = new Color(0.25f, 0.22f, 0.18f, 1f);
        
        // Item checkmark
        var itemCheckmark = new GameObject("Item Checkmark", typeof(RectTransform), typeof(Image));
        itemCheckmark.transform.SetParent(item.transform, false);
        var checkRect = itemCheckmark.GetComponent<RectTransform>();
        checkRect.anchorMin = new Vector2(0, 0.5f);
        checkRect.anchorMax = new Vector2(0, 0.5f);
        checkRect.sizeDelta = new Vector2(20, 20);
        checkRect.anchoredPosition = new Vector2(10, 0);
        
        var checkImg = itemCheckmark.GetComponent<Image>();
        checkImg.color = new Color(1f, 0.85f, 0.4f, 1f);
        
    // Item label (for options)
        var itemLabel = new GameObject("Item Label", typeof(RectTransform), typeof(Text));
        itemLabel.transform.SetParent(item.transform, false);
        var labelRect = itemLabel.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(35, 2);
        labelRect.offsetMax = new Vector2(-5, -2);
        
        var labelText = itemLabel.GetComponent<Text>();
        labelText.font = GetDefaultFont();
        labelText.color = new Color(1f, 0.95f, 0.8f, 1f);
        labelText.fontSize = 16;
        labelText.alignment = TextAnchor.MiddleLeft;
        
        // Setup toggle references
        itemToggle.graphic = itemCheckmark.GetComponent<Image>();
        
        // Setup ScrollRect
        var scrollRect = template.GetComponent<ScrollRect>();
        scrollRect.content = contentRect;
        scrollRect.viewport = viewportRect;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        
    // Assign template and texts to dropdown
        dropdown.template = templateRect;
    dropdown.captionText = capText;
    dropdown.itemText = labelText;
        
        // Gold border for dropdown
        var border = new GameObject("Border", typeof(Image));
        border.transform.SetParent(dd.transform, false);
        var borderImg = border.GetComponent<Image>();
        borderImg.color = new Color(1f, 0.85f, 0.4f, 0.5f);
        var borderRect = border.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero; borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(2, 2); borderRect.offsetMax = new Vector2(-2, -2);
        
        dropdown.options.Clear();
        foreach (var option in options)
        {
            dropdown.options.Add(new Dropdown.OptionData(option));
        }
        
        return dropdown;
    }
    
    static Dropdown CreateCompactMilitaryDropdown(Transform parent, string[] options, float width)
    {
        var dd = new GameObject("CompactDropdown", typeof(Image), typeof(Dropdown));
        dd.transform.SetParent(parent, false);
        var dropdown = dd.GetComponent<Dropdown>();
        var ddImg = dd.GetComponent<Image>();
        ddImg.color = new Color(0.15f, 0.12f, 0.08f, 1f);
        
        var layout = dd.AddComponent<LayoutElement>();
        layout.preferredWidth = width;
        
        // Create template (simplified for compact dropdown)
        var template = new GameObject("Template", typeof(RectTransform), typeof(Image));
        template.transform.SetParent(dd.transform, false);
        template.SetActive(false);
        
        var templateImg = template.GetComponent<Image>();
        templateImg.color = new Color(0.15f, 0.12f, 0.08f, 1f);
        
        var templateRect = template.GetComponent<RectTransform>();
        templateRect.anchorMin = new Vector2(0, 0);
        templateRect.anchorMax = new Vector2(1, 0);
        templateRect.anchoredPosition = new Vector2(0, 2);
        templateRect.sizeDelta = new Vector2(0, 100);
        
        // Content
        var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup));
        content.transform.SetParent(template.transform, false);
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.sizeDelta = Vector2.zero;
        
        var contentVlg = content.GetComponent<VerticalLayoutGroup>();
        contentVlg.spacing = 1;
        
        // Item template
        var item = new GameObject("Item", typeof(RectTransform), typeof(Toggle), typeof(Image));
        item.transform.SetParent(content.transform, false);
        
        var itemRect = item.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(0, 20);
        
        var itemToggle = item.GetComponent<Toggle>();
        var itemImg = item.GetComponent<Image>();
        itemImg.color = new Color(0.15f, 0.12f, 0.08f, 1f);
        
        // Item label
        var itemLabel = new GameObject("Item Label", typeof(Text));
        itemLabel.transform.SetParent(item.transform, false);
        var labelRect = itemLabel.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(5, 2);
        labelRect.offsetMax = new Vector2(-5, -2);
        
        var labelText = itemLabel.GetComponent<Text>();
        labelText.font = GetDefaultFont();
        labelText.color = Color.white;
        labelText.fontSize = 12;
        labelText.alignment = TextAnchor.MiddleCenter;
    // Caption label
    var caption = new GameObject("Label", typeof(Text));
    caption.transform.SetParent(dd.transform, false);
    var capRect = caption.GetComponent<RectTransform>();
    capRect.anchorMin = Vector2.zero; capRect.anchorMax = Vector2.one;
    capRect.offsetMin = new Vector2(6, 2); capRect.offsetMax = new Vector2(-6, -2);
    var capText = caption.GetComponent<Text>();
    capText.font = GetDefaultFont(); capText.fontSize = 12;
    capText.color = Color.white; capText.alignment = TextAnchor.MiddleCenter;

    dropdown.template = templateRect;
    dropdown.captionText = capText;
    dropdown.itemText = labelText;
        
        dropdown.options.Clear();
        foreach (var option in options)
        {
            dropdown.options.Add(new Dropdown.OptionData(option));
        }
        
        return dropdown;
    }
    
    static GameObject CreateMilitaryLabel(Transform parent, string text, float width)
    {
        var label = new GameObject("MilitaryLabel", typeof(Text));
        label.transform.SetParent(parent, false);
        var labelText = label.GetComponent<Text>();
        labelText.text = text;
        labelText.font = GetDefaultFont();
        labelText.fontSize = 20;
        labelText.fontStyle = FontStyle.Bold;
        labelText.color = new Color(1f, 0.85f, 0.4f, 1f); // Gold
        labelText.alignment = TextAnchor.MiddleLeft;
        
        var layout = label.AddComponent<LayoutElement>();
        layout.preferredWidth = width;
        
        return label;
    }
    
    static Button CreateMobileColorButton(Transform parent, Color color)
    {
        var btn = new GameObject("ColorButton", typeof(Image), typeof(Button));
        btn.transform.SetParent(parent, false);
        var img = btn.GetComponent<Image>();
        img.color = color;
        
        // Gold border
        var border = new GameObject("Border", typeof(Image));
        border.transform.SetParent(btn.transform, false);
        var borderImg = border.GetComponent<Image>();
        borderImg.color = new Color(1f, 0.85f, 0.4f, 1f);
        var borderRect = border.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero; borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(0, 0); borderRect.offsetMax = new Vector2(0, 0);
        
        var innerBorder = new GameObject("InnerBorder", typeof(Image));
        innerBorder.transform.SetParent(border.transform, false);
        var innerImg = innerBorder.GetComponent<Image>();
        innerImg.color = color;
        var innerRect = innerBorder.GetComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero; innerRect.anchorMax = Vector2.one;
        innerRect.offsetMin = new Vector2(3, 3); innerRect.offsetMax = new Vector2(-3, -3);
        
        var layout = btn.AddComponent<LayoutElement>();
        layout.preferredWidth = 60;
        
        return btn.GetComponent<Button>();
    }
    
    static Text CreateMilitaryText(Transform parent, string text)
    {
        var textObj = new GameObject("MilitaryText", typeof(Text));
        textObj.transform.SetParent(parent, false);
        var textComp = textObj.GetComponent<Text>();
        textComp.text = text;
        textComp.font = GetDefaultFont();
        textComp.fontSize = 16;
        textComp.color = new Color(0.9f, 0.9f, 0.8f, 1f); // Off-white
        textComp.alignment = TextAnchor.MiddleLeft;
        
        var layout = textObj.AddComponent<LayoutElement>();
        layout.preferredHeight = 40;
        
        return textComp;
    }

    static void CreateSkirmishTitle(Transform parent)
    {
        var titleContainer = new GameObject("TitleContainer", typeof(RectTransform), typeof(LayoutElement));
        titleContainer.transform.SetParent(parent, false);
        var titleLayout = titleContainer.GetComponent<LayoutElement>();
        titleLayout.preferredHeight = 80f;
        
        var title = new GameObject("Title", typeof(Text));
        title.transform.SetParent(titleContainer.transform, false);
        var titleText = title.GetComponent<Text>();
        titleText.text = "⚔ SKIRMISH SETUP ⚔";
        titleText.font = GetDefaultFont();
        titleText.fontSize = UIDesignSystem.Typography.TitleMedium;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = UIDesignSystem.Colors.AccentGoldLight;
        titleText.alignment = TextAnchor.MiddleCenter;
        
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = Vector2.zero;
        titleRect.anchorMax = Vector2.one;
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
    }

    static void CreateMobileMapSelection(Transform parent)
    {
        var section = UIDesignSystem.CreateSection(parent, "Map Selection");
        
        var mapRow = new GameObject("MapRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        mapRow.transform.SetParent(section.transform, false);
        var mapHlg = mapRow.GetComponent<HorizontalLayoutGroup>();
        mapHlg.spacing = UIDesignSystem.Spacing.Medium;
        mapHlg.childControlWidth = false;
        mapHlg.childControlHeight = true;
        mapHlg.childForceExpandWidth = false;
        
        var mapRowLayout = mapRow.AddComponent<LayoutElement>();
        mapRowLayout.preferredHeight = 60f;
        
        var mapLabel = new GameObject("MapLabel", typeof(Text));
        mapLabel.transform.SetParent(mapRow.transform, false);
        var labelText = mapLabel.GetComponent<Text>();
        labelText.text = "Map:";
        labelText.font = GetDefaultFont();
        labelText.fontSize = UIDesignSystem.Typography.BodyLarge;
        labelText.fontStyle = FontStyle.Bold;
        labelText.color = UIDesignSystem.Colors.AccentGoldLight;
        labelText.alignment = TextAnchor.MiddleLeft;
        
        var labelLayout = mapLabel.AddComponent<LayoutElement>();
        labelLayout.preferredWidth = 100f;
        
        var mapDropdown = UIDesignSystem.CreateMobileDropdown(mapRow.transform, new[] { 
            "Valley Forge (4 Players)", 
            "Highland Clash (4 Players)", 
            "Desert Storm (4 Players)" 
        });
        var dropdownLayout = mapDropdown.gameObject.AddComponent<LayoutElement>();
        dropdownLayout.flexibleWidth = 1f;
        dropdownLayout.preferredHeight = 50f;
    }

    static void CreateMobilePlayerSetup(Transform parent)
    {
        var section = UIDesignSystem.CreateSection(parent, "Player Setup");
        
        for (int i = 0; i < 4; i++)
        {
            CreateMobilePlayerRow(section.transform, i);
        }
        
        var resourcesSection = UIDesignSystem.CreateSection(parent, "Starting Resources");
        CreateResourcesSummary(resourcesSection.transform);
    }

    static void CreateMobilePlayerRow(Transform parent, int playerIndex)
    {
        var row = new GameObject($"Player_{playerIndex + 1}", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter), typeof(Image));
        row.transform.SetParent(parent, false);
        
        var rowImg = row.GetComponent<Image>();
        rowImg.color = new Color(0.08f, 0.12f, 0.16f, 0.6f);
        
        var vlg = row.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = UIDesignSystem.Spacing.Small;
        vlg.padding = new RectOffset((int)UIDesignSystem.Spacing.Medium, (int)UIDesignSystem.Spacing.Medium, (int)UIDesignSystem.Spacing.Small, (int)UIDesignSystem.Spacing.Small);
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        
        var rowLayout = row.AddComponent<LayoutElement>();
        rowLayout.preferredHeight = 120f;
        
        var nameRow = new GameObject("NameRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        nameRow.transform.SetParent(row.transform, false);
        var nameHlg = nameRow.GetComponent<HorizontalLayoutGroup>();
        nameHlg.spacing = UIDesignSystem.Spacing.Medium;
        nameHlg.childControlWidth = false;
        nameHlg.childControlHeight = true;
        nameHlg.childForceExpandWidth = false;
        
        var nameLayout = nameRow.AddComponent<LayoutElement>();
        nameLayout.preferredHeight = 40f;
        
        var label = new GameObject("PlayerLabel", typeof(Text));
        label.transform.SetParent(nameRow.transform, false);
        var labelText = label.GetComponent<Text>();
        labelText.text = $"⭐ PLAYER {playerIndex + 1}";
        labelText.font = GetDefaultFont();
        labelText.fontSize = UIDesignSystem.Typography.BodyLarge;
        labelText.fontStyle = FontStyle.Bold;
        labelText.color = UIDesignSystem.Colors.AccentGoldLight;
        labelText.alignment = TextAnchor.MiddleLeft;
        
        var labelLayout = label.AddComponent<LayoutElement>();
        labelLayout.preferredWidth = 150f;
        
        var colorBtn = CreateMobileColorButton(nameRow.transform, GetPlayerColor(playerIndex));
        
        var controlsRow = new GameObject("ControlsRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        controlsRow.transform.SetParent(row.transform, false);
        var controlsHlg = controlsRow.GetComponent<HorizontalLayoutGroup>();
        controlsHlg.spacing = UIDesignSystem.Spacing.Small;
        controlsHlg.childControlWidth = false;
        controlsHlg.childControlHeight = true;
        controlsHlg.childForceExpandWidth = false;
        
        var controlsLayout = controlsRow.AddComponent<LayoutElement>();
        controlsLayout.preferredHeight = 50f;
        
        var typeDropdown = CreateCompactMilitaryDropdown(controlsRow.transform, new[] { "Human", "AI Easy", "AI Normal", "AI Hard", "Disabled" }, 140f);
        var teamDropdown = CreateCompactMilitaryDropdown(controlsRow.transform, new[] { "No Team", "Team 1", "Team 2", "Team 3" }, 120f);
    }

    static void CreateResourcesSummary(Transform parent)
    {
        var resBar = new GameObject("StartingResourcesBar", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(Image));
        resBar.transform.SetParent(parent, false);
        var resImg = resBar.GetComponent<Image>();
        resImg.color = new Color(0.12f, 0.10f, 0.08f, 0.9f);
        var resHlg = resBar.GetComponent<HorizontalLayoutGroup>();
        resHlg.spacing = UIDesignSystem.Spacing.Medium;
        resHlg.padding = new RectOffset((int)UIDesignSystem.Spacing.Medium, (int)UIDesignSystem.Spacing.Medium, (int)UIDesignSystem.Spacing.Small, (int)UIDesignSystem.Spacing.Small);
        resHlg.childControlWidth = false;
        resHlg.childForceExpandWidth = false;

        void AddRes(string name, int amount)
        {
            var group = new GameObject($"{name}Group", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            group.transform.SetParent(resBar.transform, false);
            var gH = group.GetComponent<HorizontalLayoutGroup>();
            gH.spacing = UIDesignSystem.Spacing.XSmall;
            gH.childControlWidth = false;
            
            var label = new GameObject($"{name}Label", typeof(Text));
            label.transform.SetParent(group.transform, false);
            var lt = label.GetComponent<Text>();
            lt.font = GetDefaultFont();
            lt.text = name + ":";
            lt.fontSize = UIDesignSystem.Typography.BodyMedium;
            lt.color = UIDesignSystem.Colors.AccentGoldLight;
            
            var value = new GameObject($"{name}Value", typeof(Text));
            value.transform.SetParent(group.transform, false);
            var vt = value.GetComponent<Text>();
            vt.font = GetDefaultFont();
            vt.text = amount.ToString();
            vt.fontSize = UIDesignSystem.Typography.BodyMedium;
            vt.color = UIDesignSystem.Colors.TextPrimary;
        }
        
        AddRes("Wood", 1000);
        AddRes("Stone", 1000);
        AddRes("Iron", 1000);
        AddRes("Gold", 1000);
    }

    static void CreateSkirmishActionBar(Transform parent)
    {
        var actionBar = new GameObject("ActionBar", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup));
        actionBar.transform.SetParent(parent, false);
        var barRect = actionBar.GetComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0f, 0f);
        barRect.anchorMax = new Vector2(1f, 0.15f);
        barRect.offsetMin = Vector2.zero;
        barRect.offsetMax = Vector2.zero;
        var barImg = actionBar.GetComponent<Image>();
        barImg.color = new Color(0.08f, 0.06f, 0.03f, 0.98f);
        var barHlg = actionBar.GetComponent<HorizontalLayoutGroup>();
        barHlg.spacing = UIDesignSystem.Spacing.Large;
        barHlg.padding = new RectOffset((int)UIDesignSystem.Spacing.Large, (int)UIDesignSystem.Spacing.Large, (int)UIDesignSystem.Spacing.Medium, (int)UIDesignSystem.Spacing.Medium);
        barHlg.childControlWidth = true;
        barHlg.childForceExpandWidth = true;

        var backBtn = UIDesignSystem.ButtonFactory.CreateSecondaryButton(actionBar.transform, "SkirmishBackButton", "← BACK");
        var backBtnLayout = backBtn.gameObject.AddComponent<LayoutElement>();
        backBtnLayout.preferredWidth = 220f;
        
        var startBtn = CreateMilitaryButton(actionBar.transform, "StartGameButton", "⚔ START BATTLE ⚔", UIDesignSystem.Colors.ButtonDanger);
        var startLayout = startBtn.gameObject.AddComponent<LayoutElement>();
        startLayout.flexibleWidth = 1f;

        backBtn.onClick.AddListener(() => {
            var canvas = parent.root.gameObject;
            var gameModeMenu = canvas.GetComponent<GameModeMenu>();
            if (gameModeMenu != null) gameModeMenu.ShowMainGameModeSelection();
        });
    }

    static Color GetPlayerColor(int index)
    {
        Color[] colors = {
            new Color(0.2f, 0.6f, 1f, 1f),    
            new Color(1f, 0.3f, 0.3f, 1f),    
            new Color(0.3f, 0.8f, 0.3f, 1f),  
            new Color(1f, 0.8f, 0.2f, 1f),    
            new Color(0.8f, 0.3f, 0.8f, 1f),  
            new Color(1f, 0.5f, 0.1f, 1f),    
        };
        return colors[index % colors.Length];
    }

    static Button CreateMobileColorButton(Transform parent, Color color)
    {
        var btn = new GameObject("ColorButton", typeof(Image), typeof(Button));
        btn.transform.SetParent(parent, false);
        var img = btn.GetComponent<Image>();
        img.color = color;
        
        var border = new GameObject("Border", typeof(Image));
        border.transform.SetParent(btn.transform, false);
        var borderImg = border.GetComponent<Image>();
        borderImg.color = UIDesignSystem.Colors.AccentGold;
        var borderRect = border.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = Vector2.zero;
        borderRect.offsetMax = Vector2.zero;
        
        var innerBorder = new GameObject("InnerBorder", typeof(Image));
        innerBorder.transform.SetParent(border.transform, false);
        var innerImg = innerBorder.GetComponent<Image>();
        innerImg.color = color;
        var innerRect = innerBorder.GetComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero;
        innerRect.anchorMax = Vector2.one;
        innerRect.offsetMin = new Vector2(3f, 3f);
        innerRect.offsetMax = new Vector2(-3f, -3f);
        
        var layout = btn.AddComponent<LayoutElement>();
        layout.preferredWidth = 60f;
        
        return btn.GetComponent<Button>();
    }

    static Dropdown CreateCompactMilitaryDropdown(Transform parent, string[] options, float width)
    {
        var dd = new GameObject("CompactDropdown", typeof(Image), typeof(Dropdown));
        dd.transform.SetParent(parent, false);
        var dropdown = dd.GetComponent<Dropdown>();
        var ddImg = dd.GetComponent<Image>();
        ddImg.color = UIDesignSystem.Colors.ButtonSecondary;
        
        var layout = dd.AddComponent<LayoutElement>();
        layout.preferredWidth = width;
        
        var border = new GameObject("Border", typeof(Image));
        border.transform.SetParent(dd.transform, false);
        var borderImg = border.GetComponent<Image>();
        borderImg.color = UIDesignSystem.Colors.AccentGold;
        var borderRect = border.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = Vector2.zero;
        borderRect.offsetMax = Vector2.zero;
        
        var innerBg = new GameObject("InnerBackground", typeof(Image));
        innerBg.transform.SetParent(border.transform, false);
        var innerImg = innerBg.GetComponent<Image>();
        innerImg.color = UIDesignSystem.Colors.ButtonSecondary;
        var innerRect = innerBg.GetComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero;
        innerRect.anchorMax = Vector2.one;
        innerRect.offsetMin = new Vector2(2f, 2f);
        innerRect.offsetMax = new Vector2(-2f, -2f);
        
        dropdown.options.Clear();
        foreach (var option in options)
        {
            dropdown.options.Add(new Dropdown.OptionData(option));
        }
        
        return dropdown;
    }

    static Button CreateMilitaryButton(Transform parent, string name, string label, Color color)
    {
        var go = new GameObject(name, typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.color = color;
        
        var border = new GameObject("Border", typeof(Image));
        border.transform.SetParent(go.transform, false);
        var borderImg = border.GetComponent<Image>();
        borderImg.color = UIDesignSystem.Colors.AccentGold;
        var borderRect = border.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = Vector2.zero;
        borderRect.offsetMax = Vector2.zero;
        
        var innerBorder = new GameObject("InnerBorder", typeof(Image));
        innerBorder.transform.SetParent(border.transform, false);
        var innerImg = innerBorder.GetComponent<Image>();
        innerImg.color = color;
        var innerRect = innerBorder.GetComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero;
        innerRect.anchorMax = Vector2.one;
        innerRect.offsetMin = new Vector2(4f, 4f);
        innerRect.offsetMax = new Vector2(-4f, -4f);

        var textGO = new GameObject("Label", typeof(Text));
        textGO.transform.SetParent(go.transform, false);
        var t = textGO.GetComponent<Text>();
        t.text = label;
        t.alignment = TextAnchor.MiddleCenter;
        t.font = GetDefaultFont();
        t.color = UIDesignSystem.Colors.TextPrimary;
        t.fontSize = UIDesignSystem.Typography.BodyLarge;
        t.fontStyle = FontStyle.Bold;
        var tr = textGO.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero;
        tr.offsetMax = Vector2.zero;

        return go.GetComponent<Button>();
    }

    // Returns a reliable default font for editor-time UI building in Unity 6+
    static Font GetDefaultFont()
    {
        // Simple and reliable: create dynamic font from OS
        return Font.CreateDynamicFontFromOSFont("Arial", 16);
    }
}
#endif
