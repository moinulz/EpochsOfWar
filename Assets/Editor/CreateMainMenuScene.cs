#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.UI;

public static class CreateMainMenuScene
{
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
        scaler.referenceResolution = new Vector2(1080, 1920);

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
        titleTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
            img.color = new Color(0.2f, 0.3f, 0.45f, 1f);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(600, 140);

            var textGO = new GameObject("Label", typeof(Text));
            textGO.transform.SetParent(go.transform, false);
            var t = textGO.GetComponent<Text>();
            t.text = label;
            t.alignment = TextAnchor.MiddleCenter;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.color = Color.white;
            t.fontSize = 42;
            var tr = textGO.GetComponent<RectTransform>();
            tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
            tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;

            return go.GetComponent<Button>();
        }

        // Vertical button group
        var group = new GameObject("Buttons", typeof(RectTransform));
        group.transform.SetParent(canvasGO.transform, false);
        var gRect = group.GetComponent<RectTransform>();
        gRect.anchorMin = new Vector2(0.5f, 0.5f);
        gRect.anchorMax = new Vector2(0.5f, 0.5f);
        gRect.pivot = new Vector2(0.5f, 0.5f);
        gRect.anchoredPosition = new Vector2(0, -100);
        gRect.sizeDelta = new Vector2(650, 700);

        var playBtn = MakeButton(group.transform, "PlayButton", "Play");
        var settingsBtn = MakeButton(group.transform, "SettingsButton", "Settings");
        var quitBtn = MakeButton(group.transform, "QuitButton", "Quit");

        // Position buttons
        playBtn.transform.localPosition = new Vector3(0, 160, 0);
        settingsBtn.transform.localPosition = new Vector3(0, 0, 0);
        quitBtn.transform.localPosition = new Vector3(0, -160, 0);

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
        stText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
        pl.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
        
        // Setup main camera for top-down view
        var mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.transform.position = new Vector3(0, 25, -15); // Higher and further back
            mainCam.transform.rotation = Quaternion.Euler(60, 0, 0); // Steeper angle
            mainCam.fieldOfView = 60f; // Wider field of view
            mainCam.farClipPlane = 1000f; // Ensure we can see far objects
            mainCam.gameObject.AddComponent<CameraController>();
        }
        
        // Create ground plane
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(50, 1, 50); // 500x500 units (much larger)
        
        // Create a simple material for the ground
        var groundMaterial = new Material(Shader.Find("Standard"));
        groundMaterial.color = new Color(0.3f, 0.6f, 0.3f, 1f); // Green ground
        ground.GetComponent<Renderer>().material = groundMaterial;
        
        // Try to place Capital prefab
        var capitalPrefabPath = "Assets/Prefabs/Buildings/Capital.prefab";
        var capitalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(capitalPrefabPath);
        if (capitalPrefab != null)
        {
            var capital = PrefabUtility.InstantiatePrefab(capitalPrefab) as GameObject;
            capital.transform.position = new Vector3(0, 0, 0); // Place on ground
            capital.transform.localScale = new Vector3(10, 10, 10); // Make it much larger
            capital.name = "Capital";
        }
        else
        {
            // Fallback: create a large cube if prefab not found
            var capital = GameObject.CreatePrimitive(PrimitiveType.Cube);
            capital.name = "Capital (Placeholder)";
            capital.transform.position = new Vector3(0, 5f, 0); // Elevated so it's visible
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
        gameScaler.referenceResolution = new Vector2(1080, 1920);
        
        // Create mobile control buttons
        CreateMobileControls(gameCanvas.transform, mainCam?.gameObject.GetComponent<CameraController>());
        
        // Add Game Manager
        var gameManager = new GameObject("GameManager");
        var gameMgr = gameManager.AddComponent<GameManager>();
        if (mainCam != null)
        {
            gameMgr.gameCamera = mainCam;
            gameMgr.cameraController = mainCam.gameObject.GetComponent<CameraController>();
        }
        
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
        var list = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(mmPath, true),
            new EditorBuildSettingsScene(gamePath, true)
        };
        EditorBuildSettings.scenes = list;

        EditorUtility.DisplayDialog("Main Menu", "Main Menu + Game scenes created and added to Build Settings.", "OK");

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
            lt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
            lt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
            lt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
        
        void CreateMobileControls(Transform parent, CameraController cameraController)
        {
            if (cameraController == null) return;
            
            // Create control panel background
            var controlPanel = new GameObject("ControlPanel", typeof(Image));
            controlPanel.transform.SetParent(parent, false);
            var panelImg = controlPanel.GetComponent<Image>();
            panelImg.color = new Color(0, 0, 0, 0.5f);
            var panelRect = controlPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0);
            panelRect.anchorMax = new Vector2(1, 0.3f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            // Movement buttons (WASD layout)
            var upBtn = CreateControlButton(controlPanel.transform, "Up", new Vector2(0, 120), "W");
            var downBtn = CreateControlButton(controlPanel.transform, "Down", new Vector2(0, -120), "S");
            var leftBtn = CreateControlButton(controlPanel.transform, "Left", new Vector2(-120, 0), "A");
            var rightBtn = CreateControlButton(controlPanel.transform, "Right", new Vector2(120, 0), "D");
            
            // Zoom buttons
            var zoomInBtn = CreateControlButton(controlPanel.transform, "Zoom In", new Vector2(400, 60), "+");
            var zoomOutBtn = CreateControlButton(controlPanel.transform, "Zoom Out", new Vector2(400, -60), "-");
            
            // Wire up button events
            upBtn.onClick.AddListener(() => cameraController.MoveUp());
            downBtn.onClick.AddListener(() => cameraController.MoveDown());
            leftBtn.onClick.AddListener(() => cameraController.MoveLeft());
            rightBtn.onClick.AddListener(() => cameraController.MoveRight());
            zoomInBtn.onClick.AddListener(() => cameraController.ZoomIn());
            zoomOutBtn.onClick.AddListener(() => cameraController.ZoomOut());
            
            // Add instructions text
            var instructions = new GameObject("Instructions", typeof(Text));
            instructions.transform.SetParent(parent, false);
            var instText = instructions.GetComponent<Text>();
            instText.text = "WASD: Move Camera | Mouse Scroll: Zoom | Touch: Pan & Pinch to Zoom";
            instText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            instText.fontSize = 28;
            instText.color = Color.white;
            instText.alignment = TextAnchor.MiddleCenter;
            var instRect = instructions.GetComponent<RectTransform>();
            instRect.anchorMin = new Vector2(0, 1);
            instRect.anchorMax = new Vector2(1, 1);
            instRect.anchoredPosition = new Vector2(0, -50);
            instRect.sizeDelta = new Vector2(0, 60);
        }
        
        Button CreateControlButton(Transform parent, string name, Vector2 position, string label)
        {
            var btn = new GameObject(name, typeof(Image), typeof(Button));
            btn.transform.SetParent(parent, false);
            var img = btn.GetComponent<Image>();
            img.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
            var rect = btn.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 100);
            rect.anchoredPosition = position;
            
            var labelGO = new GameObject("Label", typeof(Text));
            labelGO.transform.SetParent(btn.transform, false);
            var labelText = labelGO.GetComponent<Text>();
            labelText.text = label;
            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            labelText.fontSize = 32;
            labelText.color = Color.white;
            labelText.alignment = TextAnchor.MiddleCenter;
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            
            return btn.GetComponent<Button>();
        }
    }
}
#endif