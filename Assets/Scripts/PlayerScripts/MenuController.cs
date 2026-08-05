using Unity.AppUI.UI;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject HUDUI;

    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void Start()
    {
        pauseMenuUI.SetActive(false);
        HUDUI.SetActive(true);
        GameFlags.instance.SetFlag("isGamePaused", false);
    }

    private void Update()
    {
        if (inputActions != null)
        {
            if (inputActions.Player.Menu.WasPressedThisFrame())
            {
                if (GameFlags.instance.GetFlag("isGamePaused"))
                {
                    OnResumeGame();
                }
                else
                {
                    OnPauseGame();
                }
            }
        }
    }

    public void OnPauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
        FreezePlayer();
        pauseMenuUI.SetActive(true);
        HUDUI.SetActive(false);
        GameFlags.instance.SetFlag("isGamePaused", true);
    }

    public void OnResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
        UnfreezePlayer();
        pauseMenuUI.SetActive(false);
        HUDUI.SetActive(true);
        GameFlags.instance.SetFlag("isGamePaused", false);
    }

    public void OnSaveGame()
    {
        Debug.Log("saved (Experimental)");
    }

    public void OnQuitGame()
    {
        Time.timeScale = 1f;
        PlayerHealth.Instance.ReturnToMainMenu();
    }

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

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }
}
