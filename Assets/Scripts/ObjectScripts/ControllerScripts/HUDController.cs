using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController instance;

    [SerializeField] private Canvas HUD;

    [SerializeField] private GameObject toolbar;
    [SerializeField] private GameObject mainInventory;

    [SerializeField] private Image cursor;
    [SerializeField] private GameObject health;
    [SerializeField] private GameObject stamina;

    void Awake()
    {
        instance = this;
    }

    [SerializeField] private TMP_Text interactionText;

    public void hideUINote ()
    {
        cursor.enabled = false;
        health.SetActive(false);
        stamina.SetActive(false);
    }

    public void showUiNote()
    {
        cursor.enabled = true;
        health.SetActive(true);
        stamina.SetActive(true);
    }

    public void disableInventory()
    {
        toolbar.SetActive(false);
        mainInventory.SetActive(false);
    }

    public void enableInventory()
    {
        toolbar.SetActive(true);
    }

    public void ShowInteractionText()
    {
        interactionText.gameObject.SetActive(true);
    }



    public void DisableInteractionText()
    {
        interactionText.gameObject.SetActive(false);
    }
}