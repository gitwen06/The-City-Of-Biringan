using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SaveManager : MonoBehaviour
{
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
            //get save path (folder/savefile(index).json)
            string path = GetSavePath(i);

            if (File.Exists(path))
            {
                //getsavepath(index(i)) -> read -> json -> savadata
                SaveData fullData = ReadDisk(i);
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
}
