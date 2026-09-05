using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameFlags : MonoBehaviour
{
    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    public static GameFlags instance;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
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

        foreach (KeyValuePair<string, bool> flag in flags)
        {
            Debug.Log($"GameFlags: {flag.Key} = {flag.Value}");
        }
    }

    public bool GetFlag(string key)
    {
        bool value;
        bool wasFound = flags.TryGetValue(key, out value);

        foreach (KeyValuePair<string, bool> flag in flags)
        {
            Debug.Log($"GameFlags: {flag.Key} = {flag.Value}");
        }

        return wasFound ? value : false;
    }

    //create entry -> populate entry -> add entry to list -> return list(Read by SaveManager)
    public List<FlagEntry> GetFlagsSaveData()
    {
        List<FlagEntry> flagEntries = new List<FlagEntry>();

        for (int i = 0; i < flags.Count; i++)
        {
            FlagEntry entry = new FlagEntry();
            entry.flagName = flags.ElementAt(i).Key;
            entry.flagValue = flags.ElementAt(i).Value;
            flagEntries.Add(entry);
        }
        return flagEntries;
    }

    //populate flags from parameter(Called by SaveManager)
    public void LoadFlagsFromSaveData(List<FlagEntry> flagEntries)
    {
        flags.Clear();
        foreach (FlagEntry entry in flagEntries)
        {
            flags[entry.flagName] = entry.flagValue;
        }
    }
}
