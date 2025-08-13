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

        // Background panel
        var bg = new GameObject("Background", typeof(Image));
        bg.transform.SetParent(canvasGO.transform, false);
        var bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero; bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;  bgRect.offsetMax = Vector2.zero;
        bg.GetComponent<Image>().color = new Color(0.05f, 0.08f, 0.12f, 1f);

        // Title
    var title = new GameObject("Title", typeof(Text));
        title.transform.SetParent(canvasGO.transform, false);
        var titleTxt = title.GetComponent<Text>();
        titleTxt.text = "Epochs of War";
        titleTxt.alignment = TextAnchor.MiddleCenter;
    titleTxt.font = GetDefaultFont();
        titleTxt.fontSize = 72;
        titleTxt.color = Color.white;
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
            // Higher contrast background for readability
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
        
        // Background
        var bg = new GameObject("Background", typeof(Image));
        bg.transform.SetParent(canvasGO.transform, false);
        var bgImage = bg.GetComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.2f, 1f);
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
        var title = new GameObject("SkirmishTitle", typeof(Text));
        title.transform.SetParent(parent, false);
        var titleText = title.GetComponent<Text>();
        titleText.text = "SKIRMISH SETUP";
    titleText.font = GetDefaultFont();
        titleText.fontSize = 48;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.85f);
        titleRect.anchorMax = new Vector2(1, 0.95f);
        titleRect.offsetMin = Vector2.zero; titleRect.offsetMax = Vector2.zero;
        
        // Create a scroll view for the skirmish setup (simplified for now)
        var scrollArea = new GameObject("ScrollArea", typeof(RectTransform));
        scrollArea.transform.SetParent(parent, false);
        var scrollRect = scrollArea.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.1f, 0.1f);
        scrollRect.anchorMax = new Vector2(0.9f, 0.8f);
        scrollRect.offsetMin = Vector2.zero; scrollRect.offsetMax = Vector2.zero;
        
        // Coming soon text (actual skirmish setup would be more complex)
        var comingSoon = new GameObject("ComingSoonText", typeof(Text));
        comingSoon.transform.SetParent(scrollArea.transform, false);
        var csText = comingSoon.GetComponent<Text>();
        csText.text = "SKIRMISH SETUP COMING SOON!\\n\\nThis will include:\\n- Map Selection\\n- Player Configuration\\n- AI Difficulty Settings\\n- Resource Settings\\n- Victory Conditions";
    csText.font = GetDefaultFont();
        csText.fontSize = 32;
        csText.color = Color.white;
        csText.alignment = TextAnchor.MiddleCenter;
        var csRect = comingSoon.GetComponent<RectTransform>();
        csRect.anchorMin = Vector2.zero; csRect.anchorMax = Vector2.one;
        csRect.offsetMin = Vector2.zero; csRect.offsetMax = Vector2.zero;
        
        var backBtn = MakeButton(parent, "SkirmishBackButton", "BACK");
        SetButtonPosition(backBtn, new Vector2(0.5f, 0.05f), new Vector2(200, 50));
    }
    
    // Helper methods for UI creation
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

    // Returns a reliable default font for editor-time UI building in Unity 6+
    static Font GetDefaultFont()
    {
        // Unity 6: Try multiple paths for built-in fonts
        Font font = null;
        
        // Method 1: Try AssetDatabase.GetBuiltinExtraResource with LegacyRuntime (Unity 6 preferred)
        try { font = AssetDatabase.GetBuiltinExtraResource<Font>("LegacyRuntime.ttf"); } catch { }
        if (font != null) return font;
        
        // Method 2: Try EditorGUIUtility.Load
        try { font = EditorGUIUtility.Load("LegacyRuntime.ttf") as Font; } catch { }
        if (font != null) return font;
        
        // Method 3: Try AssetDatabase with exception handling
        try { font = AssetDatabase.GetBuiltinExtraResource<Font>("LegacyRuntime.ttf"); } catch { }
        if (font != null) return font;
        
        // Method 4: Create dynamic font from OS (guaranteed to work)
        try 
        { 
            font = Font.CreateDynamicFontFromOSFont("Arial", 16);
            if (font != null && font.name != "Arial") // Check if it actually loaded
                return font;
        } 
        catch { }
        
        // Method 5: Last resort - use Unity's default GUI font
        try { return GUI.skin.font; } catch { }
        
        // Absolutely final fallback
        return Font.CreateDynamicFontFromOSFont("Arial", 16);
    }
}
#endif
