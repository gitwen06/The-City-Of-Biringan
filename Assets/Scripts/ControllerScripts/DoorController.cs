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
        animator = GetComponent<Animator>();
        outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
        DoorRegistry.instance.RegisterDoor(doorName, this);

        if (GameFlags.instance.GetFlag(doorName + "_unlocked"))
        {
            doorLocked = false;
        }

        isDoorOpen = GameFlags.instance.GetFlag(doorName + "_open");
        animator.SetBool("isOpen", isDoorOpen);
    }

    public void Interact()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).length >
         animator.GetCurrentAnimatorStateInfo(0).normalizedTime) { return; }

        if (doorLocked)
        {
            CoreInventoryController.instance.UseSelectedItem();
        }

        if (!doorLocked)
        {
            isDoorOpen = !isDoorOpen;
            animator.SetBool("isOpen", isDoorOpen);
            GameFlags.instance.SetFlag(doorName + "_open", isDoorOpen);
        }
        else
        {
            NotificationController.instance.ShowNotification("Door Locked");
        }
    }

    public void unlockDoor(string doorName)
    {
        doorLocked = false;
        GameFlags.instance.SetFlag(doorName + "_unlocked", true);
        Debug.Log($"Unlocked door {doorName} and gameflag is {GameFlags.instance.GetFlag(doorName + "_unlocked")}");
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
