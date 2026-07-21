using UnityEngine;
using System.Collections.Generic;

public class GameFlags : MonoBehaviour
{
    public Dictionary<string , bool> flags = new Dictionary<string , bool>();

    public static GameFlags instance;

    public void Awake()
    {
        instance = this;
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
}
