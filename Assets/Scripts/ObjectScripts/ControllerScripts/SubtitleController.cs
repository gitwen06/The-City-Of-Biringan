using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class SubtitleController : MonoBehaviour
{
    [SerializeField] private List<string> subtitleTexts;
    [SerializeField] private List<float> duration;

    [SerializeField] private GameObject blackBoxes;
    [SerializeField] private TextMeshProUGUI textField;

    [SerializeField] private float minCharDelay = 0.05f;
    [SerializeField] private float maxCharDelay = 0.15f;

    private Coroutine currentSubtitleCoroutine;

    private void Awake()
    {
        textField.text = "";
    }

    public void ShowLineIndex(int index)
    {
        if (index < 0 || index >= subtitleTexts.Count || index >= duration.Count)
        {
            Debug.LogWarning($"SubtitleController: index {index} is out of range.");
            return;
        }

        ShowLine(subtitleTexts[index], duration[index]);
        blackBoxes.gameObject.SetActive(true);
    }

    private void ShowLine(string text, float holdDuration)
    {
        if (currentSubtitleCoroutine != null)
        {
            StopCoroutine(currentSubtitleCoroutine);
        }


        currentSubtitleCoroutine = StartCoroutine(TypeSubtitle(text, holdDuration));
    }

    private IEnumerator TypeSubtitle(string text, float holdDuration)
    {

        textField.text = "";
        foreach (char c in text)
        {
            textField.text += c;
            yield return new WaitForSeconds(Random.Range(minCharDelay, maxCharDelay));
        }

        yield return new WaitForSeconds(holdDuration);

        textField.text = "";
        blackBoxes.gameObject.SetActive(false);
        currentSubtitleCoroutine = null;
    }
}