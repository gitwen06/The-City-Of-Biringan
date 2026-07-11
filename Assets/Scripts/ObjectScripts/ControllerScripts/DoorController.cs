using UnityEngine;

public class DoorController : MonoBehaviour, Iinteractable
{
    private Animator animator;
    private bool isDoorOpen = false;
    private Outline outline;


    void Start()
    {
        // Get the Animator component attached to the pivot object
        animator = GetComponent<Animator>();
        outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
    }

    // Call this method via player interaction system (Raycast, Trigger, or UI button)
    public void Interact()
    {
        isDoorOpen = !isDoorOpen;

        // Update the Animator parameter to shift states
        animator.SetBool("isOpen", isDoorOpen);
    }

    public void EnableOutline()
    {
        if (outline != null) outline.enabled = true;
    }

    public void DisableOutline()
    {
        if (outline != null) outline.enabled = false;
    }
}
