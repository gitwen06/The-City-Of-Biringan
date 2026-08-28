using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{

    public Vector3 playerPosition;
    public Quaternion playerRotation;

    public Vector3 cameraPosition;
    public Quaternion cameraRotation;

    public float health;
    public float stamina;

    public string sceneName;
    public int sceneBuildIndex;

    public List<InventoryEntry> inventory;

    public SaveSlot metaData;
}
