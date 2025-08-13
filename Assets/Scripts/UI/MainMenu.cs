using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Wire by editor tool")]
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;
    public GameObject settingsPanel;
    public SceneLoader sceneLoader;
    public string gameSceneName = "Game";

    void Start()
    {
        if (playButton) playButton.onClick.AddListener(OnPlay);
        if (settingsButton) settingsButton.onClick.AddListener(ToggleSettings);
        if (quitButton) quitButton.onClick.AddListener(OnQuit);

#if UNITY_ANDROID
        // On Android, hide Quit by default (Google’s UX guidance)
        if (quitButton) quitButton.gameObject.SetActive(false);
#endif
    }

    void OnPlay()
    {
        // Load the Game Mode selection scene instead of going directly to game
        if (sceneLoader) sceneLoader.LoadSceneAsync("GameModeSelection");
        else UnityEngine.SceneManagement.SceneManager.LoadScene("GameModeSelection");
    }

    void ToggleSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}