using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class AsyncLoader : MonoBehaviour
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private int targetSceneIndex = 2;

    void Awake()
    {
        Time.timeScale = 1f; // Ensure time scale is reset to normal when loading a new scene
    }
    void Start()
    {
        StartCoroutine(LoadAsynchronously(targetSceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.value = progress;
            yield return null;
        }
    }
}
