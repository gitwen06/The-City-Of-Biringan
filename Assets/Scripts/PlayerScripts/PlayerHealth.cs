using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Canvas HUD;
    [SerializeField] private Canvas deathHUD;

    [SerializeField] private float maxHealth;

    private float currentHealth;

    public static PlayerHealth instance;

    public void Awake()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        GameFlags.instance.SetFlag("isPlayerDead", false);
    }

    public float ReduceHP(float damage)
    {
        if(currentHealth - damage <= 0)
        {
            GameFlags.instance.SetFlag("isPlayerDead", true);
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
        if(currentHealth > maxHealth) { return currentHealth; }

        currentHealth += heal;
        healthSlider.value = currentHealth;
        return currentHealth;
    }

    public void ShowDeathHUD()
    {
        GameFlags.instance.SetFlag("isPlayerDead", true);
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

    public float GetHealth()
    {
        return currentHealth;
    }

    public void SetHealth(float value)
    {
        currentHealth = value;
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
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
