using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("Optional loading UI")]
    public CanvasGroup loadingGroup;
    public Slider progressBar;
    public Text progressLabel;

    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadRoutine(sceneName));
    }

    IEnumerator LoadRoutine(string sceneName)
    {
        if (loadingGroup)
        {
            loadingGroup.gameObject.SetActive(true);
            loadingGroup.alpha = 1f;
            if (progressBar) progressBar.value = 0f;
            if (progressLabel) progressLabel.text = "0%";
        }

        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            float p = Mathf.Clamp01(op.progress / 0.9f);
            if (progressBar) progressBar.value = p;
            if (progressLabel) progressLabel.text = Mathf.RoundToInt(p * 100f) + "%";
            yield return null;
        }

        // 0.9f reached → finalize bar
        if (progressBar) progressBar.value = 1f;
        if (progressLabel) progressLabel.text = "100%";
        yield return new WaitForSeconds(0.2f);

        op.allowSceneActivation = true;
    }
}
