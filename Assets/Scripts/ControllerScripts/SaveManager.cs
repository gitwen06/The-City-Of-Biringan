using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;

    private List<SaveSlot> SaveMetadataSlots = new List<SaveSlot>();

    public static SaveManager instance;

    private const int MAX_SLOTS = 5;

    private int currentActiveSlot = -1;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        StartCoroutine(AutosaveLoop());
        Debug.Log("Autosave started : 5:00");
    }
    public void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void PerformAutosave()
    {
        int targetSlot = (currentActiveSlot == -1) ? FindNextFreeSlot() : currentActiveSlot;
        if (targetSlot == -1) { return; }

        SaveSlot metadata = new SaveSlot();
        metadata.saveName = "Autosave";
        metadata.saveDate = System.DateTime.Now.ToString();
        metadata.slotIndex = targetSlot;
        metadata.isAutosave = true;

        SaveData data = CaptureCurrentState(metadata);
        SaveToSlot(targetSlot, data);

        Debug.Log($"[Autosave] Saved to slot {targetSlot}");
    }

    public void SaveToSlot(int slotIndex, SaveData data)
    {
        WriteDisk(slotIndex, data);
        currentActiveSlot = slotIndex;
    }

    public SaveData LoadFromSlot(int slotIndex)
    {
        SaveData data = ReadDisk(slotIndex);
        currentActiveSlot = slotIndex;
        return data;
    }

    public List<SaveSlot> GetAllSaveMetaData()
    {
        //alat of comments on this since this is confusing for me somehow

        List<SaveSlot> result = new List<SaveSlot>();

        for (int i = 0; i < MAX_SLOTS; i++)
        {
            //getsavepath(index(i)) -> read -> json -> savadata
            SaveData fullData = ReadDisk(i);

            if (fullData != null)
            {
                //metaData is a SaveSlot in SaveData class.
                result.Add(fullData.metaData);
            }
        }

        return result;
    }

    private string GetSavePath(int slotIndex)
    {
        return $"{Application.persistentDataPath}/saveslot{slotIndex}.json"; //e.g. path/saveslot0.json
    }

    public void WriteDisk(int slotIndex, SaveData data)
    {
        //get save path
        string path = GetSavePath(slotIndex);
        //SaveData -> Json
        string json = JsonUtility.ToJson(data);
        //Write to json
        Debug.Log("[SaveManager] Save Written to JSON");
        File.WriteAllText(path, json);
    }

    public SaveData ReadDisk(int slotIndex)
    {
        //get path
        string path = GetSavePath(slotIndex);

        if (File.Exists(path))
        {
            //read all text from JSON
            string contents = File.ReadAllText(path);
            //Json -> SaveData
            SaveData data = JsonUtility.FromJson<SaveData>(contents);
            return data;
        }
        else
        {
            Debug.Log($"no save file at {path}");
            return null;
        }
    }

    public void DeleteDisk(int slotIndex)
    {
        string path = GetSavePath(slotIndex);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public void RenameFile(int slotIndex, string newName)
    {
        SaveData data = ReadDisk(slotIndex);
        if (data == null) { return; }
        data.metaData.saveName = newName;
        WriteDisk(slotIndex, data);
    }

    public int FindNextFreeSlot()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            string path = GetSavePath(i);
            if (!File.Exists(path))
            {
                Debug.Log("[SaveManager] slots :");
                return i;
            }
        }
        return -1;
    }

    public SaveData CaptureCurrentState(SaveSlot metaData)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        int sceneBuildIndex = currentScene.buildIndex;

        float health = PlayerHealth.instance.GetHealth();
        float stamina = PlayerMovement.instance.GetStamina();

        Transform playerPosT = PlayerMovement.instance.GetPlayerTransform();
        Vector3 playerPos = playerPosT.position;
        Quaternion playerRot = playerPosT.rotation;

        Transform playerRotT = PlayerMovement.instance.GetPlayerCameraTransform();
        Vector3 playerCamPos = playerRotT.position;
        Quaternion playerCamRot = playerRotT.rotation;

        List<InventoryEntry> inventory = CoreInventoryController.instance.GetInventorySaveData();

        SaveData data = new SaveData();
        data.sceneName = sceneName;
        data.sceneBuildIndex = sceneBuildIndex;
        data.health = health;
        data.stamina = stamina;
        data.playerPosition = playerPos;
        data.playerRotation = playerRot;
        data.cameraPosition = playerCamPos;
        data.cameraRotation = playerCamRot;
        data.inventory = inventory;
        data.metaData = metaData;

        metaData.generalArea = sceneName;
        metaData.questName = "Placeholder";

        return data;
    }

    public IEnumerator LoadSlotAndApply(int slotindex)
    {
        SaveData data = LoadFromSlot(slotindex);
        if (data == null) { yield break; }

        AsyncOperation operation = SceneManager.LoadSceneAsync(data.sceneName);
        while (!operation.isDone)
        {
            yield return null;
        }

        Transform playerT = PlayerMovement.instance.GetPlayerTransform();
        playerT.position = data.playerPosition;
        playerT.rotation = data.playerRotation;

        Transform camT = PlayerMovement.instance.GetPlayerCameraTransform();
        camT.position = data.cameraPosition;
        camT.rotation = data.cameraRotation;

        PlayerHealth.instance.SetHealth(data.health);
        PlayerMovement.instance.SetStamina(data.stamina);

        foreach (InventoryEntry entry in data.inventory)
        {
            ItemScriptableObject item = itemDatabase.GetItemById(entry.itemId);
            CoreInventoryController.instance.SetItemAtSlot(entry.slotIndex, item, entry.quantity);
        }

        Time.timeScale = 1f;
        PlayerMovement.instance.UnfreezeInput();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private IEnumerator AutosaveLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(300f);
            Debug.Log("[Autosave] Saved Game!");
            PerformAutosave();
        }
    }
}