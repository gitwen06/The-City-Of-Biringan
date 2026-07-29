using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemScriptableObject : ScriptableObject
{
    public string itemName;
    public string id;
    public Sprite Icon;
    public string description;
    public int maxStackSize;

    public bool useableInOpenSpace;

    public GameObject handModel;

    public virtual void Use()
    {
        Debug.Log("USED");
    }
}
