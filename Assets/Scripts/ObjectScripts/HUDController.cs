using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public static HUDController instance;
    
    void Awake()
    {
        instance = this;
    }

    [SerializeField] private TMP_Text interactionText;

    public void EnableInteractionText()
    {
        interactionText.gameObject.SetActive(true);
    }
    public void DisableInteractionText()
    {
        interactionText.gameObject.SetActive(false);
    }
}
