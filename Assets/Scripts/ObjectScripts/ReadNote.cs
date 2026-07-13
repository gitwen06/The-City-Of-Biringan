using UnityEngine;

public class ReadNote : MonoBehaviour, Iinteractable
{
    [SerializeField]public string text;
    Outline outline;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
    }

    public void Interact()
    {
        // Open the note UI. NoteController will handle cursor and freezing.
        NoteController.instance.SetText(text, true);
        HUDController.instance.hideUINote();
        HUDController.instance.DisableInteractionText();

    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }
}
