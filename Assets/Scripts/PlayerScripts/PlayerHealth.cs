using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Canvas HUD;
    [SerializeField] private Canvas deathHUD;

    [SerializeField] private float maxHealth;
    private float currentHealth;

    public void Awake()
    {
        Instance = this;
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    private void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

    }

    public float ReduceHP(float damage)
    {
        if(currentHealth - damage <= 0)
        {
            ShowDeathHUD();
        }
        else
        {
            currentHealth -= damage;
            healthSlider.value = currentHealth;
        }
        return currentHealth;
    }

    public float HealHP(float heal)
    {
        currentHealth += heal;
        healthSlider.value = currentHealth;
        return currentHealth;
    }

    public void ShowDeathHUD()
    {
        HUD.gameObject.SetActive(false);
        deathHUD.gameObject.SetActive(true);
        FreezePlayer();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

    public void FreezePlayer()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Use the newer API to find any object of type PlayerMovement
        PlayerMovement pm = Object.FindAnyObjectByType<PlayerMovement>();
        if (pm != null)
        {
            pm.FreezeInput();
        }
    }
}
