using UnityEngine;

[CreateAssetMenu(fileName = "New Key", menuName = "Inventory/Key")]
public class KeyItem : ItemScriptableObject
{
    public string doorToOpen;

    public override void Use()
    {
        DoorController door = DoorRegistry.instance.GetDoor(doorToOpen);

        if (door != null)
        {
            door.unlockDoor();
        }
    }
}

