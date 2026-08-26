using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    private List<SaveSlot> SaveMetadataSlots = new List<SaveSlot>();

    public static SaveManager instance;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public List<SaveSlot> GetAllSaveMetaData()
    {
        return SaveMetadataSlots;
    }
}
