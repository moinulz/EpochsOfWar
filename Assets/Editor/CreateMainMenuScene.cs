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
        titleRect.anchoredPosition = new Vector2(0, -200);
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
        gRect.anchoredPosition = Vector2.zero;
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

        // Quality dropdown
        Dropdown qualityDropdown = MakeLabeledDropdown(panel.transform, "Graphics Quality", new Vector2(0, 200));
        // Music slider
        Slider musicSlider = MakeLabeledSlider(panel.transform, "Music Volume", new Vector2(0, 40), 0f, 1f, 0.5f);
        // Vibration toggle
        Toggle vibToggle = MakeLabeledToggle(panel.transform, "Vibration (Mobile)", new Vector2(0, -120), true);

        // Close button
        var closeBtn = MakeButton(panel.transform, "CloseButton", "Close");
        closeBtn.transform.localPosition = new Vector3(0, -300, 0);

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

        // Wire up UI interactions
        closeBtn.onClick.AddListener(() => settingsPanel.SetActive(false));

        // Save MainMenu scene
        var mmPath = "Assets/Scenes/MainMenu.unity";
        System.IO.Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), mmPath);

        // Create placeholder Game scene
        var gameScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        var goLabel = new GameObject("Label", typeof(Text));
        var t = goLabel.GetComponent<Text>();
        t.text = "Game Scene (placeholder)";
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        var tr = goLabel.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;
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
            var ddr = dd.GetComponent<RectTransform>();
            ddr.sizeDelta = new Vector2(380, 80);
            return dd.GetComponent<Dropdown>();
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

            var tg = new GameObject("Toggle", typeof(Toggle));
            tg.transform.SetParent(root.transform, false);
            var t = tg.GetComponent<Toggle>();
            t.isOn = value;
            var tr = tg.GetComponent<RectTransform>();
            tr.sizeDelta = new Vector2(80, 80);
            return t;
        }
    }
}
#endif