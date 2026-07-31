using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar;
    public TextMeshProUGUI progressText;
    public GameObject loadingUI;

    void Start()
    {
        StartCoroutine(LoadSceneAsync(SceneLoader.SceneToLoad));
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingUI.SetActive(true);
        progressBar.value = 0f;
        progressText.text = "0%";

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

            if (operation.progress >= 0.9f)
            {
                progressText.text = "100%";
                yield return new WaitForSeconds(0.2f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}