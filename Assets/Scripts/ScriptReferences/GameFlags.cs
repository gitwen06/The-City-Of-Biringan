using UnityEngine;
using System.Collections.Generic;

public class GameFlags : MonoBehaviour
{
    private Dictionary<string , bool> flags = new Dictionary<string , bool>();

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
    }

    public bool GetFlag(string key)
    {
        bool value;
        bool wasFound = flags.TryGetValue(key, out value);
        return wasFound ? value : false;
    }
}
