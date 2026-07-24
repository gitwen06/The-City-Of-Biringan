using UnityEngine;

public interface Iinteractable
{
    void Interact();
    void EnableOutline();
    void DisableOutline();
}

public class CoreInteraction : MonoBehaviour
{
    public float playerReach = 3.0f;
    Iinteractable currentInteractable;
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
        if (DialogueController.instance.IsDialogueActive()) { return; }
        CheckInteraction();
        if (inputActions.Player.Interact.WasPressedThisFrame())
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
                Debug.Log("Interacted");
            }
            else
            {
                CoreInventoryController.instance.UseSelectedItem();
            }
        }
    }

    void CheckInteraction()
    {
        RaycastHit hit;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out hit, playerReach))
        {
            Iinteractable newInteractable = hit.collider.GetComponent<Iinteractable>();

            if (newInteractable != null)
            {
                SetNewCurrentInteractable(newInteractable);
                return;
            }
        }

        DisableCurrentInteractable();
    }

    void SetNewCurrentInteractable(Iinteractable newInteractable)
    {
        if (currentInteractable == newInteractable) return;

        DisableCurrentInteractable();
        currentInteractable = newInteractable;
        currentInteractable.EnableOutline();
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
}