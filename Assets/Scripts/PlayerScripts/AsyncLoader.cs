using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class AsyncLoader : MonoBehaviour
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private int targetSceneIndex = 2;

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
