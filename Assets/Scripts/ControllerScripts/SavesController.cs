using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SavesController : MonoBehaviour
{
    [SerializeField] private GameObject saveHUD;
    [SerializeField] private GameObject saveSlotBG;

    [SerializeField] private GameObject newSlot;

    [SerializeField] private GameObject renameUI;
    [SerializeField] private TMP_InputField renameInputField;

    [SerializeField] private GameObject renameBtn;
    [SerializeField] private GameObject deleteBtn;

    private enum PopupMode { None, Rename, NewSave }
    private PopupMode currentPopupMode = PopupMode.None;

    private string renameName;

    private List<SaveSlot> saves = new List<SaveSlot>();
    private List<SaveSlotUI> saveSlotUI = new List<SaveSlotUI>();
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
        selectedSlotIndex = slotIndex;

        foreach (var slotUI in saveSlotUI)
        {
            if (slotUI.GetSlotIndex() == slotIndex)
            {
                slotUI.Select();
            }
            else
            {
                slotUI.Deselect();
            }
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
            LayoutRebuilder.ForceRebuildLayoutImmediate(saveSlotBG.GetComponent<RectTransform>());
            slotUI.Setup(saves[i], saves[i].slotIndex, this);
            saveSlotUI.Add(slotUI);
        }

        newSlot.transform.SetAsLastSibling();

        // if saves less than 5, show the new slot button or gameobject whatever, else hide it
        UpdateNewSlotVisibility();
        UpdateSecondaryButtonsVisibility();
    }

    public void OnHide()
    {
        saveHUD.SetActive(false);
        selectedSlotIndex = -1;
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

    public void OnPopUpSubmit()
    {
        if (currentPopupMode == PopupMode.Rename)
        {
            ConfirmRename();
        }
        else if (currentPopupMode == PopupMode.NewSave)
        {
            ConfirmNewSave();
        }
    }

    public void ConfirmNewSave()
    {
        string newName = renameInputField.text;
        if (string.IsNullOrEmpty(newName)) { return; }

        int index = SaveManager.instance.FindNextFreeSlot();
        if (index == -1) { return; }

        SaveSlot metadata = new SaveSlot();
        metadata.saveName = newName;
        metadata.saveDate = System.DateTime.Now.ToString();
        metadata.slotIndex = index;
        metadata.isAutosave = false;

        SaveData data = SaveManager.instance.CaptureCurrentState(metadata);
        SaveManager.instance.SaveToSlot(index, data);

        currentPopupMode = PopupMode.None;
        renameUI.SetActive(false);
        PopulateSlots();
    }

    public void ConfirmRename()
    {
        string newName = renameInputField.text;
        if (!string.IsNullOrEmpty(newName))
        {
            SaveManager.instance.RenameFile(selectedSlotIndex, newName);
            currentPopupMode = PopupMode.None;
            PopulateSlots();
            renameUI.SetActive(false);
        }
    }

    public void CancelRename()
    {
        renameUI.SetActive(false);
        currentPopupMode = PopupMode.None;
    }

    public void OnNewSlotClicked()
    {
        currentPopupMode = PopupMode.NewSave;
        renameUI.SetActive(true);
        renameInputField.text = "";
    }

    //Called from "Load" btn
    public void OnLoadClicked()
    {
        if (selectedSlotIndex == -1) { return; }
        SaveManager.instance.StartCoroutine(SaveManager.instance.LoadSlotAndApply(selectedSlotIndex));
    }

    //Called from "Save" btn
    public void OnSaveClicked()
    {
        //only appears when selected slot != -1.
        //call capturecurrentstate in savemanager to get savedata
        //overwrite current selected savefile if selectedSlotIndex != -1, else create new savefile
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (selectedSlotIndex == -1 || sceneName == "MainMenuScene") { return; }

        SaveData existing = SaveManager.instance.ReadDisk(selectedSlotIndex);

        SaveSlot metadata = new SaveSlot();
        metadata.saveName = (existing != null) ? existing.metaData.saveName : "Save File " + selectedSlotIndex;
        metadata.saveDate = System.DateTime.Now.ToString();
        metadata.slotIndex = selectedSlotIndex;
        metadata.isAutosave = false;

        SaveData data = SaveManager.instance.CaptureCurrentState(metadata);
        SaveManager.instance.SaveToSlot(selectedSlotIndex, data);

        PopulateSlots();
    }

    //Called from "Rename" btn
    public void OnRenameClicked()
    {
        if (selectedSlotIndex == -1) { return; }
        //only appears when selected slot != -1;
        renameUI.SetActive(true);
        // renameName = inputfield text
        //wait for user to click either confirm or cancel.
        //if confirm, use renameName to rename saveslot., if cancel, toggle visibility, reset renameName to empty string
        currentPopupMode = PopupMode.Rename;
        renameUI.SetActive(true);
        renameInputField.text = "";
    }

    //called from "Delete"btn
    public void OnDeleteClicked()
    {
        if (selectedSlotIndex == -1) { return; }

        SaveManager.instance.DeleteDisk(selectedSlotIndex);

        selectedSlotIndex = -1;

        PopulateSlots();
    }
}