using UnityEngine;
using UnityEngine.Playables;
using Unity.Cinemachine;

public class CutsceneRunner : MonoBehaviour
{
    public static CutsceneRunner instance;

    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private Transform playerCameraTransform;

    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    private bool isPlaying = false;
    private PlayableDirector currentDirector;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public void Play(PlayableDirector director)
    {
        if (isPlaying) { return; }
        Debug.Log("CutsceneRunner: Play called with director: " + director.name);

        HUDController.instance.hideUINote();
        HUDController.instance.DisableInteractionText();
        HUDController.instance.disableInventory();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        FreezePlayer();

        isPlaying = true;
        currentDirector = director;

        originalCameraPosition = playerCameraTransform.localPosition;
        originalCameraRotation = playerCameraTransform.localRotation;

        brain.enabled = true;
        currentDirector.stopped += OnCutsceneStopped;
        currentDirector.Play();
    }

    private void OnCutsceneStopped(PlayableDirector director)
    {
        Debug.Log("OnCutsceneStopped fired");
        HUDController.instance.showUiNote();
        HUDController.instance.enableInventory();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UnfreezePlayer();
        brain.enabled = false;

        playerCameraTransform.localPosition = originalCameraPosition;
        playerCameraTransform.localRotation = originalCameraRotation;

        currentDirector.stopped -= OnCutsceneStopped;
        currentDirector = null;
        isPlaying = false;
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
}