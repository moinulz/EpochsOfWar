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

        // Canvas
        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
    var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    scaler.referenceResolution = new Vector2(1920, 1080);

        // Background panel with gradient
        var bg = new GameObject("Background", typeof(Image));
        bg.transform.SetParent(canvasGO.transform, false);
        var bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero; bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;  bgRect.offsetMax = Vector2.zero;
        bg.GetComponent<Image>().color = new Color(0.02f, 0.05f, 0.08f, 1f); // Deep dark blue

        // Title with modern styling
        var title = new GameObject("Title", typeof(Text));
        title.transform.SetParent(canvasGO.transform, false);
        var titleTxt = title.GetComponent<Text>();
        titleTxt.text = "EPOCHS OF WAR";
        titleTxt.alignment = TextAnchor.MiddleCenter;
        titleTxt.font = GetDefaultFont();
        titleTxt.fontSize = 84;
        titleTxt.fontStyle = FontStyle.Bold;
        titleTxt.color = new Color(0.9f, 0.95f, 1f, 1f); // Slightly blue-tinted white
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -150);
        titleRect.sizeDelta = new Vector2(900, 160);

        // Button helper
        Button MakeButton(Transform parent, string name, string label)
        {
            var go = new GameObject(name, typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var img = go.GetComponent<Image>();
            // Modern button styling with subtle gradient
            img.color = new Color(0.1f, 0.15f, 0.25f, 0.9f);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(600, 140);

            // Add subtle border effect
            var border = new GameObject("Border", typeof(Image));
            border.transform.SetParent(go.transform, false);
            var borderImg = border.GetComponent<Image>();
            borderImg.color = new Color(0.3f, 0.4f, 0.6f, 0.4f);
            var borderRect = border.GetComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero; borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = new Vector2(2, 2); borderRect.offsetMax = new Vector2(-2, -2);

            var textGO = new GameObject("Label", typeof(Text));
            textGO.transform.SetParent(go.transform, false);
            var t = textGO.GetComponent<Text>();
            t.text = label;
            t.alignment = TextAnchor.MiddleCenter;
            t.font = GetDefaultFont();
            t.color = new Color(0.9f, 0.9f, 1f, 1f);
            t.fontSize = 48;
            t.fontStyle = FontStyle.Bold;
            var tr = textGO.GetComponent<RectTransform>();
            tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
            tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;

            return go.GetComponent<Button>();
        }

    // Vertical button group with layout
    var group = new GameObject("Buttons", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        group.transform.SetParent(canvasGO.transform, false);
        var gRect = group.GetComponent<RectTransform>();
    gRect.anchorMin = new Vector2(0.5f, 0.5f);
    gRect.anchorMax = new Vector2(0.5f, 0.5f);
        gRect.pivot = new Vector2(0.5f, 0.5f);
    gRect.anchoredPosition = new Vector2(0, -80);
    gRect.sizeDelta = new Vector2(700, 0);

    var vlg = group.GetComponent<VerticalLayoutGroup>();
    vlg.spacing = 24f;
    vlg.childAlignment = TextAnchor.MiddleCenter;
    vlg.childControlWidth = true;
    vlg.childControlHeight = true;
    vlg.childForceExpandWidth = false;
    vlg.childForceExpandHeight = false;
    vlg.padding = new RectOffset(0, 0, 0, 0);

    var fitter = group.GetComponent<ContentSizeFitter>();
    fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
    fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var playBtn = MakeButton(group.transform, "PlayButton", "Play");
        var settingsBtn = MakeButton(group.transform, "SettingsButton", "Settings");
        var quitBtn = MakeButton(group.transform, "QuitButton", "Quit");

        // Let VerticalLayoutGroup handle positioning; ensure consistent size
        foreach (var b in new[] { playBtn, settingsBtn, quitBtn })
        {
            var br = b.GetComponent<RectTransform>();
            br.sizeDelta = new Vector2(600, 120);
        }

        // Settings panel (hidden)
        var settingsPanel = new GameObject("SettingsPanel", typeof(Image));
        settingsPanel.transform.SetParent(canvasGO.transform, false);
        var spImg = settingsPanel.GetComponent<Image>();
        spImg.color = new Color(0f, 0f, 0f, 0.66f);
        var spRect = settingsPanel.GetComponent<RectTransform>();
        spRect.anchorMin = Vector2.zero; spRect.anchorMax = Vector2.one;
        spRect.offsetMin = Vector2.zero; spRect.offsetMax = Vector2.zero;
        settingsPanel.SetActive(false);

        // Settings content: panel + controls
        var panel = new GameObject("Panel", typeof(Image));
        panel.transform.SetParent(settingsPanel.transform, false);
        panel.GetComponent<Image>().color = new Color(0.12f, 0.18f, 0.25f, 1f);
        var pRect = panel.GetComponent<RectTransform>();
        pRect.anchorMin = new Vector2(0.5f, 0.5f);
        pRect.anchorMax = new Vector2(0.5f, 0.5f);
        pRect.sizeDelta = new Vector2(800, 900);
        pRect.anchoredPosition = Vector2.zero;

        // Settings title
        var settingsTitle = new GameObject("SettingsTitle", typeof(Text));
        settingsTitle.transform.SetParent(panel.transform, false);
        var stText = settingsTitle.GetComponent<Text>();
        stText.text = "Settings";
        stText.alignment = TextAnchor.MiddleCenter;
    stText.font = GetDefaultFont();
        stText.fontSize = 48;
        stText.color = Color.white;
        var stRect = settingsTitle.GetComponent<RectTransform>();
        stRect.sizeDelta = new Vector2(700, 80);
        stRect.anchoredPosition = new Vector2(0, 380);

        // Quality dropdown
        Dropdown qualityDropdown = MakeLabeledDropdown(panel.transform, "Graphics Quality", new Vector2(0, 200));
        // Music slider
        Slider musicSlider = MakeLabeledSlider(panel.transform, "Music Volume", new Vector2(0, 40), 0f, 1f, 0.5f);
        // Vibration toggle
        Toggle vibToggle = MakeLabeledToggle(panel.transform, "Vibration (Mobile)", new Vector2(0, -120), true);

        // Close button
        var closeBtn = MakeButton(panel.transform, "CloseButton", "Close");
        closeBtn.transform.localPosition = new Vector3(0, -350, 0);
        var closeBtnRect = closeBtn.GetComponent<RectTransform>();
        closeBtnRect.sizeDelta = new Vector2(400, 100);

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
        
        // Create Canvas
        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvasComp = canvasGO.GetComponent<Canvas>();
        canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        
        // Background with modern dark theme
        var bg = new GameObject("Background", typeof(Image));
        bg.transform.SetParent(canvasGO.transform, false);
        var bgImage = bg.GetComponent<Image>();
        bgImage.color = new Color(0.02f, 0.05f, 0.08f, 1f);
        var bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero; bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero; bgRect.offsetMax = Vector2.zero;
        
        // Title
        var title = new GameObject("Title", typeof(Text));
        title.transform.SetParent(canvasGO.transform, false);
        var titleText = title.GetComponent<Text>();
        titleText.text = "SELECT GAME MODE";
    titleText.font = GetDefaultFont();
        titleText.fontSize = 60;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.8f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero; titleRect.offsetMax = Vector2.zero;
        
        // Create main buttons
        var buttonGroup = new GameObject("ButtonGroup", typeof(RectTransform));
        buttonGroup.transform.SetParent(canvasGO.transform, false);
        var groupRect = buttonGroup.GetComponent<RectTransform>();
        groupRect.anchorMin = new Vector2(0.2f, 0.3f);
        groupRect.anchorMax = new Vector2(0.8f, 0.7f);
        groupRect.offsetMin = Vector2.zero; groupRect.offsetMax = Vector2.zero;
        
        // Campaign button
        var campaignBtn = MakeButton(buttonGroup.transform, "CampaignButton", "CAMPAIGN");
        SetButtonPosition(campaignBtn, new Vector2(0, 0.75f), new Vector2(400, 80));
        
        // Skirmish button
        var skirmishBtn = MakeButton(buttonGroup.transform, "SkirmishButton", "SKIRMISH");
        SetButtonPosition(skirmishBtn, new Vector2(0, 0.5f), new Vector2(400, 80));
        
        // Multiplayer button
        var multiplayerBtn = MakeButton(buttonGroup.transform, "MultiplayerButton", "MULTIPLAYER");
        SetButtonPosition(multiplayerBtn, new Vector2(0, 0.25f), new Vector2(400, 80));
        
        // Back button
        var backBtn = MakeButton(buttonGroup.transform, "BackButton", "BACK TO MAIN MENU");
        SetButtonPosition(backBtn, new Vector2(0, 0f), new Vector2(400, 80));
        
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
        
        // Wire up back button to return to main game mode selection
        backBtn.onClick.AddListener(() => {
            var gameModeMenu = parent.GetComponentInParent<GameModeMenu>();
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
        // Title
        var title = new GameObject("SkirmishTitle", typeof(Text));
        title.transform.SetParent(parent, false);
        var titleText = title.GetComponent<Text>();
        titleText.text = "SKIRMISH SETUP";
        titleText.font = GetDefaultFont();
        titleText.fontSize = 48;
        titleText.color = new Color(0.9f, 0.9f, 1f, 1f);
        titleText.alignment = TextAnchor.MiddleCenter;
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.9f);
        titleRect.anchorMax = new Vector2(1, 0.98f);
        titleRect.offsetMin = Vector2.zero; titleRect.offsetMax = Vector2.zero;
        
        // Main content area with scroll
        var scrollArea = new GameObject("ScrollArea", typeof(RectTransform), typeof(ScrollRect), typeof(Image));
        scrollArea.transform.SetParent(parent, false);
        var scrollRect = scrollArea.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.05f, 0.12f);
        scrollRect.anchorMax = new Vector2(0.95f, 0.85f);
        scrollRect.offsetMin = Vector2.zero; scrollRect.offsetMax = Vector2.zero;
        
        var scrollImg = scrollArea.GetComponent<Image>();
        scrollImg.color = new Color(0.08f, 0.12f, 0.18f, 0.95f);
        
        var scroll = scrollArea.GetComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        
        // Content container
        var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(scrollArea.transform, false);
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.anchoredPosition = Vector2.zero;
        
        var vlg = content.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = 20f;
        vlg.padding = new RectOffset(30, 30, 20, 20);
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        
        var fitter = content.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        scroll.content = contentRect;
        
        // Map Selection Section
        CreateSection(content.transform, "MAP SELECTION", (sectionContent) => {
            var mapDropdown = CreateStyledDropdown(sectionContent, "Map", new[] { "Default Map (4 Players)", "Small Map (2 Players)", "Large Map (6 Players)" });
            var mapInfo = CreateInfoText(sectionContent, "A balanced map with strategic choke points and resource distribution.");
        });
        
        // Player Setup Section
        CreateSection(content.transform, "PLAYERS", (sectionContent) => {
            for (int i = 0; i < 4; i++)
            {
                CreatePlayerRow(sectionContent, i);
            }
        });
        
        // Game Options Section
        CreateSection(content.transform, "GAME OPTIONS", (sectionContent) => {
            var victoryDropdown = CreateStyledDropdown(sectionContent, "Victory Condition", new[] { "Destroy All Capitals", "Eliminate All Units", "Last Standing", "Custom" });
            var resourcesToggle = CreateStyledToggle(sectionContent, "Custom Starting Resources", false);
        });
        
        // Action buttons at bottom
        var buttonArea = new GameObject("ButtonArea", typeof(RectTransform));
        buttonArea.transform.SetParent(parent, false);
        var buttonRect = buttonArea.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.1f, 0.02f);
        buttonRect.anchorMax = new Vector2(0.9f, 0.1f);
        buttonRect.offsetMin = Vector2.zero; buttonRect.offsetMax = Vector2.zero;
        
        var startBtn = CreateModernButton(buttonArea.transform, "StartGameButton", "START GAME", new Color(0.2f, 0.6f, 0.3f, 1f));
        var startBtnRect = startBtn.GetComponent<RectTransform>();
        startBtnRect.anchorMin = new Vector2(0.55f, 0.2f);
        startBtnRect.anchorMax = new Vector2(0.95f, 0.8f);
        startBtnRect.offsetMin = Vector2.zero; startBtnRect.offsetMax = Vector2.zero;
        
        var backBtn = CreateModernButton(buttonArea.transform, "SkirmishBackButton", "BACK", new Color(0.4f, 0.4f, 0.4f, 1f));
        var backBtnRect = backBtn.GetComponent<RectTransform>();
        backBtnRect.anchorMin = new Vector2(0.05f, 0.2f);
        backBtnRect.anchorMax = new Vector2(0.35f, 0.8f);
        backBtnRect.offsetMin = Vector2.zero; backBtnRect.offsetMax = Vector2.zero;
        
        // Wire up back button
        backBtn.onClick.AddListener(() => {
            var gameModeMenu = parent.GetComponentInParent<GameModeMenu>();
            if (gameModeMenu != null) gameModeMenu.ShowMainGameModeSelection();
        });
        
        // Add SkirmishSetup component
        var skirmishSetup = scrollArea.AddComponent<SkirmishSetup>();
        skirmishSetup.startGameButton = startBtn;
        skirmishSetup.backButton = backBtn;
    }
    
    static void CreateSection(Transform parent, string title, System.Action<Transform> createContent)
    {
        var section = new GameObject($"Section_{title}", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter), typeof(Image));
        section.transform.SetParent(parent, false);
        
        var sectionImg = section.GetComponent<Image>();
        sectionImg.color = new Color(0.12f, 0.16f, 0.22f, 0.8f);
        
        var sectionVlg = section.GetComponent<VerticalLayoutGroup>();
        sectionVlg.spacing = 10f;
        sectionVlg.padding = new RectOffset(20, 20, 15, 15);
        sectionVlg.childControlWidth = true;
        sectionVlg.childControlHeight = false;
        sectionVlg.childForceExpandWidth = true;
        
        var sectionFitter = section.GetComponent<ContentSizeFitter>();
        sectionFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // Section title
        var titleObj = new GameObject("Title", typeof(Text));
        titleObj.transform.SetParent(section.transform, false);
        var titleText = titleObj.GetComponent<Text>();
        titleText.text = title;
        titleText.font = GetDefaultFont();
        titleText.fontSize = 28;
        titleText.color = new Color(0.8f, 0.9f, 1f, 1f);
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleLeft;
        
        var titleLayout = titleObj.AddComponent<LayoutElement>();
        titleLayout.preferredHeight = 35;
        
        createContent(section.transform);
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

    // Returns a reliable default font for editor-time UI building in Unity 6+
    static Font GetDefaultFont()
    {
        // Simple and reliable: create dynamic font from OS
        return Font.CreateDynamicFontFromOSFont("Arial", 16);
    }
}
#endif
