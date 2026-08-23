using System.Collections.Generic; 
using Unity.AppUI.UI;
using UnityEngine;


public class SavesController : MonoBehaviour
{
    [SerializeField] private GameObject SaveHUD;
    [SerializeField] private GameObject SaveSlotParent;
    [SerializeField] private Button backBtn;

    [SerializeField] private Button NewSave;

    public List<SaveSlot> Saves = new List<SaveSlot>();
    public int selectedSlotIndex = -1;
    public static SavesController instance;

    [SerializeField] private SaveSlotUI saveSlotUIprefab;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void SelectSlot(int slotIndex)
    {
        Debug.Log(slotIndex);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void OnHide()
    {
        SaveHUD.SetActive(false);
    }
}
