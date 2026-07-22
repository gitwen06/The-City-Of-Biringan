using UnityEngine;
using TMPro;

public class NoteController : MonoBehaviour
{
    public static NoteController instance;

    public bool isreading = false;

    string text;
    public TMP_Text noteText;
    bool isShowing = false;
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // ensure we have a reference to the TMP_Text. Prefer inspector assign, fall back to GetComponent.
        if (noteText == null)
            noteText = GetComponent<TMP_Text>();

        // ensure UI initially reflects state
        if (noteText != null)
        {
            noteText.text = text;
            noteText.gameObject.SetActive(isShowing);
        }
        else
        {
            Debug.LogWarning("NoteController: noteText TMP_Text reference is missing.", this);
        }
    }

    public void SetMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Freeze player input (called when opening a note)
    public void FreezePlayer()
    {
        // Use the newer API to find any object of type PlayerMovement
        PlayerMovement pm = Object.FindAnyObjectByType<PlayerMovement>();
        if (pm != null)
        {
            pm.FreezeInput();
        }
    }

    // Unfreeze player input (call this when closing the note UI)
    public void UnfreezePlayer()
    {
        // Use the newer API to find any object of type PlayerMovement
        PlayerMovement pm = Object.FindAnyObjectByType<PlayerMovement>();
        if (pm != null)
        {
            pm.UnfreezeInput();
        }
    }

    

    private void OnDisable()
    {
        if (noteText != null)
        {
            noteText.text = "";
            noteText.gameObject.SetActive(false);
        }
    }

    public void SetText(string newText, bool Active)
    {
        // toggle note visibility if node is clicked again
        isShowing = Active;

        if (noteText == null)
        {
            Debug.LogWarning("NoteController: noteText is null when trying to SetText.", this);
            return;
        }

        noteText.text = newText;
        noteText.gameObject.SetActive(isShowing);

        if (isShowing)
        {
            // opening note: show cursor and unlock so UI can receive clicks
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isreading = true;

            // freeze player input while viewing
            FreezePlayer();
        }
        else
        {
            // closing note: hide cursor and re-lock
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isreading = false;
            Debug.Log($"isreading is now: {isreading}");

            // unfreeze player input
            UnfreezePlayer();
            // restore HUD when closing note
            if (HUDController.instance != null) HUDController.instance.showUiNote();
        }

        
    }

    public bool isReading()
    { 
        return isreading;
    }
    // Called by UI button to close the note
    public void CloseNote()
    {
        SetText("", false);
        isreading = false;
    }
}
