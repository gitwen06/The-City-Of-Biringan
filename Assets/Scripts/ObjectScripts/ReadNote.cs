using UnityEngine;
using TMPro;

public class ReadNote : MonoBehaviour, Iinteractable
{
    [SerializeField]public string text;
    Outline outline;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        outline = GetComponent<Outline>();
        
    }

    public void Interact()
    {
        HUDController.instance.SerializeNoteText(text);
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
