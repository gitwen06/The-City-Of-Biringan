using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public static HUDController instance;


    void Awake()
    {
        instance = this;
    }

    [SerializeField] private TMP_Text interactionText;


    public void SerializeNoteText(string text)
    {
        NoteController.instance.SetText(text);
    }


    public void ShowInteractionText()
    {
        interactionText.gameObject.SetActive(true);
    }

    public void DisableInteractionText()
    {
        interactionText.gameObject.SetActive(false);
    }
}