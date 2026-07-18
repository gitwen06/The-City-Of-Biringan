using UnityEngine;
using System.Collections.Generic;

public class DoorRegistry : MonoBehaviour
{
    public static DoorRegistry instance;

    private Dictionary<string, DoorController> registry = new Dictionary<string, DoorController>();

    public void Awake()
    {
        instance = this;
    }

    public void RegisterDoor(string doorId, DoorController door)
    {
        registry[doorId] = door;
        
    }

    public DoorController GetDoor(string id)
    {
        DoorController foundDoor;
        bool WasFound = registry.TryGetValue(id, out foundDoor);

        return WasFound ? foundDoor : null;
    }

}
