using UnityEngine;

public class DoorController : MonoBehaviour, Iinteractable
{
    private Animator animator;
    private bool isDoorOpen = false;
    private Outline outline;

    public string doorName; //doorid

    public bool doorLocked = false;
    void Start()
    {
        // Get the Animator component attached to the pivot object
        animator = GetComponent<Animator>();
        outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
        DoorRegistry.instance.RegisterDoor(doorName, this);
    }

    // Call this method via player interaction system (Raycast, Trigger, or UI button)
    public void Interact()
    {
        if (doorLocked)
        {
            CoreInventoryController.instance.UseSelectedItem();
        }

        if (!doorLocked)
        {
            isDoorOpen = !isDoorOpen;
            animator.SetBool("isOpen", isDoorOpen);
        }
        else
        {
            NotificationController.instance.ShowNotification("Door Locked");
        }
    }

    public void unlockDoor()
    {
        doorLocked = false;
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
