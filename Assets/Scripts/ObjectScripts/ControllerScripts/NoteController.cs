using UnityEngine;
using TMPro;

public class NoteController : MonoBehaviour
{
    public static NoteController instance;

    string text;
    public TMP_Text noteText;   
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        noteText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        noteText.text = text;
    }

    private void DisableClearText()
    {

    }

    private void OnDisable()
    {
        noteText.text = "";
    }

    public string SetText(string newText)
    {
        text = newText;
        return text;
    }


}
