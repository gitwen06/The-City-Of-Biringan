using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class SavesController : MonoBehaviour
{
    [SerializeField] private GameObject saveHUD;
    [SerializeField] private GameObject saveSlotBG;

    [SerializeField] private GameObject newSlot;

    [SerializeField] private GameObject renameUI;
    [SerializeField] private TMP_InputField renameInputField;
    [SerializeField] private GameObject renameBtn;
    [SerializeField] private GameObject deleteBtn;

    private string renameName;

    private List<SaveSlot> saves = new List<SaveSlot>();
    private List<SaveSlotUI> saveSlotUI = new List<SaveSlotUI>();
    public int selectedSlotIndex = -1;
    public int prevSlotIndex = -1;
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
    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void UpdateNewSlotVisibility()
    {
        if (saves.Count < 5)
        {
            newSlot.SetActive(true);

        }
        else
        {
            newSlot.SetActive(false);
        }
    }

    public void UpdateSecondaryButtonsVisibility()
    {
        bool hasSelection = selectedSlotIndex != -1;
        renameBtn.SetActive(hasSelection);
        deleteBtn.SetActive(hasSelection);
    }

    public void SelectSlot(int slotIndex)
    {
        prevSlotIndex = selectedSlotIndex;
        selectedSlotIndex = slotIndex;
        // deselect previous slot if it exists (if not -1)
        if (prevSlotIndex != -1 && prevSlotIndex < saveSlotUI.Count)
        {
            saveSlotUI[prevSlotIndex].Deselect();
        }
        // select new slot
        if (selectedSlotIndex != -1 && selectedSlotIndex < saveSlotUI.Count)
        {
            saveSlotUI[selectedSlotIndex].Select();
        }

        UpdateSecondaryButtonsVisibility();
    }

    public void PopulateSlots()
    {
        //make sure to clear the previous slots before populating new ones
        foreach (var slotUI in saveSlotUI)
        {
            Destroy(slotUI.gameObject);
        }
        saveSlotUI.Clear();
        saves.Clear();

        //populate the list of saves from the SaveManager
        //Saves = SaveManager.instance.GetAllSaveMetaData();
        foreach (var data in SaveManager.instance.GetAllSaveMetaData())
        {
            saves.Add(data);
        }

        //populate the UI elements for each save slot
        for (int i = 0; i < saves.Count; i++)
        {
            SaveSlotUI slotUI = Instantiate(saveSlotUIprefab, saveSlotBG.transform);
            slotUI.Setup(saves[i], i, this);
            saveSlotUI.Add(slotUI);
        }

        // if saves less than 5, show the new slot button or gameobject whatever, else hide it
        UpdateNewSlotVisibility();
    }

    public void OnHide()
    {
        saveHUD.SetActive(false);
        selectedSlotIndex = -1;
        prevSlotIndex = -1;
        foreach (var slotUI in saveSlotUI)
        {
            slotUI.Deselect();
            Destroy(slotUI.gameObject);
        }
        saveSlotUI.Clear();
        saves.Clear();

        UpdateNewSlotVisibility();
        UpdateSecondaryButtonsVisibility();
    }

    //Called from "Load" btn
    public void OnLoadClicked()
    {
        if(selectedSlotIndex == -1) { return; }
        Debug.Log($"load slot {selectedSlotIndex}");
    }

    //Called from "Save" btn
    public void OnSaveClicked()
    {
        if (selectedSlotIndex == -1) { return; }
        //only appears when selected slot != -1.
        //overwrite current selected savefile if selectedSlotIndex != -1, else create new savefile

    }

    //Called from "Rename" btn
    public void OnRenameClicked()
    {
        //only appears when selected slot != -1;
        renameUI.SetActive(true);
        // renameName = inputfield text
        //wait for user to click either confirm or cancel.
        //if confirm, use renameName to rename saveslot., if cancel, toggle visibility, reset renameName to empty string

        renameName = "";
    }

    //called from "Delete"btn
    public void OnDeleteClicked()
    {
        if (selectedSlotIndex == -1) { return; }

        saves.RemoveAt(selectedSlotIndex);

        Destroy(saveSlotUI[selectedSlotIndex].gameObject);
        saveSlotUI.RemoveAt(selectedSlotIndex);

        for (int i = 0; i < saveSlotUI.Count; i++)
        {
            saveSlotUI[i].UpdateIndex(i); //resync index of each slotUI after deletion
        }

        selectedSlotIndex = -1;

        UpdateNewSlotVisibility();
        UpdateSecondaryButtonsVisibility();
    }
}
