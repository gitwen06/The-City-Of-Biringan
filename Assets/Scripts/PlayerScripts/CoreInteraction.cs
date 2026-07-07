using UnityEngine;

public class CoreInteraction : MonoBehaviour
{
    public float playerReach = 3.0f;
    Interactable currentInteractable;
    [SerializeField] private Camera playerCamera;

    private InputSystem_Actions inputActions;


    private void Awake()
    {
        inputActions = new InputSystem_Actions();
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
        if(inputActions.Player.Interact.WasPressedThisFrame() && currentInteractable != null)
        {
            currentInteractable.Interact();
            Debug.Log("workds");
        }
    }

    void CheckInteraction()
    {
        RaycastHit hit;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if(Physics.Raycast(ray, out hit, playerReach)) 
        {
            if(hit.collider.tag == "Interactable")
            {
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();

                if (newInteractable.enabled)
                {
                    SetNewCurrentInteractable(newInteractable);
                }
                // if interactable is disabled, disable the current interactable
                else
                {
                    DisableCurrentInteractable();
                }
            }
            //if not an interactable, disable the current interactable
            else
            {
                DisableCurrentInteractable();
            }
        }
        // if not hitting anything, disable the current interactable
        else
        {
            DisableCurrentInteractable();
        }
    }

    void SetNewCurrentInteractable(Interactable newInteractable)
    {
        currentInteractable = newInteractable;
        currentInteractable.EnableOutline();
        HUDController.instance.EnableInteractionText();
    }

    void DisableCurrentInteractable()
    {
        HUDController.instance.DisableInteractionText();
        if (currentInteractable)
        {
            currentInteractable.DisableOutline();
            currentInteractable = null;
        }
    }



}
