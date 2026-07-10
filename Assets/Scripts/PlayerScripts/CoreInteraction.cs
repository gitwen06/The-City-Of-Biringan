using UnityEngine;

public class CoreInteraction : MonoBehaviour
{
    public float playerReach = 3.0f;
    Interactable currentInteractable;
    DoorController currentDoor;
    [SerializeField] private Camera playerCamera;

    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void Start()
    {
        HUDController.instance.DisableInteractionText();
    }
    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }
    private void Update()
    {
        CheckInteraction();
        if (inputActions.Player.Interact.WasPressedThisFrame())
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
                Debug.Log("Interactable interacted");
            }
            else if (currentDoor != null)
            {
                currentDoor.ToggleDoor();
                Debug.Log("Door toggled");
            }
        }
    }

    void CheckInteraction()
    {
        RaycastHit hit;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out hit, playerReach))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();

                if (newInteractable != null && newInteractable.enabled)
                {
                    DisableCurrentDoor();
                    SetNewCurrentInteractable(newInteractable);
                    return;
                }
            }
            else if (hit.collider.CompareTag("isDoor"))
            {
                DoorController door = hit.collider.GetComponent<DoorController>();

                if (door != null)
                {
                    DisableCurrentInteractable();
                    SetNewCurrentDoor(door);
                    return;
                }
            }
        }

        // Nothing valid is being looked at: ray missed entirely, hit an
        // unrelated collider, or hit a disabled/component-less target.
        // Clear both focuses every time we land here.
        DisableCurrentInteractable();
        DisableCurrentDoor();
    }

    void SetNewCurrentInteractable(Interactable newInteractable)
    {
        if (currentInteractable == newInteractable) return;

        currentInteractable = newInteractable;
        currentInteractable.EnableOutline();
        if (HUDController.instance != null)
            HUDController.instance.ShowInteractionText();
    }

    void SetNewCurrentDoor(DoorController door)
    {
        if (currentDoor == door) return;

        currentDoor = door;
        currentDoor.EnableOutline();
        if (HUDController.instance != null)
            HUDController.instance.ShowInteractionText();
    }

    void DisableCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable.DisableOutline();
            currentInteractable = null;
            if (HUDController.instance != null)
                HUDController.instance.DisableInteractionText();
        }
    }

    void DisableCurrentDoor()
    {
        if (currentDoor != null)
        {
            currentDoor.DisableOutline();
            currentDoor = null;
            if (HUDController.instance != null)
                HUDController.instance.DisableInteractionText();
        }
    }
}