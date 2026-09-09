using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameFlags : MonoBehaviour
{
    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    public static GameFlags instance;

    private InputSystem_Actions inputActions;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        inputActions = new InputSystem_Actions();
    }

    public void Update()
    {
        if (inputActions.Player.AddFlagTest1.WasPressedThisFrame())
        {
            SetFlag("TestFlag", true);
            Debug.Log("[GameFlags] TestFlag set to true.");
        }
        if (inputActions.Player.AddFlagTest2.WasPressedThisFrame())
        {
            SetFlag("TestFlag", false);
            Debug.Log("[GameFlags] TestFlag set to false.");
        }
        if (inputActions.Player.StartTestCoroutine.WasPressedThisFrame())
        {
            StartCoroutine(Start());
        }


    }

    IEnumerator Start()
    {
        while (true) { 
            yield return new WaitForSeconds(5.0f);
            foreach (var kvp in flags)
            {
                Debug.Log($"[GameFlags] Flag: {kvp.Key}, Value: {kvp.Value}");
            }
        }
    }

    public void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void SetFlag(string key, bool value)
    {
        flags[key] = value;
    }

    public bool GetFlag(string key)
    {
        bool value;
        bool wasFound = flags.TryGetValue(key, out value);

        return wasFound ? value : false;
    }

    //create entry -> populate entry -> add entry to list -> return list(Read by SaveManager)
    public List<FlagEntry> GetFlagsSaveData()
    {
        List<FlagEntry> flagEntries = new List<FlagEntry>();

        foreach (var kvp in flags)
        {
            FlagEntry entry = new FlagEntry();
            entry.flagName = kvp.Key;
            entry.flagValue = kvp.Value;
            flagEntries.Add(entry);
        }

        return flagEntries;
    }

    //populate flags from parameter(Called by SaveManager)
    public void LoadFlagsFromSaveData(List<FlagEntry> flagEntries)
    {
        flags.Clear(); // clear existing flags before loading new ones

        foreach (FlagEntry entry in flagEntries)
        {
            SetFlag(entry.flagName, entry.flagValue);
        }
        Debug.Log("[GameFlags] Flags loaded from save data.");
        foreach (var kvp in flags)
        {
            Debug.Log($"[GameFlags] Flag: {kvp.Key}, Value: {kvp.Value}");
        }
    }
}
